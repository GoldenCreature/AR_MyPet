using System;
using System.Collections;
using UnityEngine;
using SGMG.AR_MyPet;
using HJS.AR_MyPet;

namespace CMS.AR_MyPet
{
    /// <summary>
    /// singleton 구조로 작성
    /// 수치 감소/증가 로직을 담당, UI와 Animator에 변화를 전달
    /// MyPetManager의 OnPetTouched, ReportHealthChanged와 연동
    /// </summary>
    public class PetStatusController : MonoBehaviour
    {
        // ─── Singleton ───
        public static PetStatusController Instance { get; private set; }

        // ─── 상태값 ───
        [Header("초기값 설정")]
        [SerializeField, Range(0f, 100f)] private float initialHunger    = 80f;
        [SerializeField, Range(0f, 100f)] private float initialIntimacy  = 50f;
        [SerializeField, Range(0f, 100f)] private float initialHappiness = 70f;

        [Header("시간 경과 감소율 (초당)")]
        [SerializeField] private float hungerDecayRate    = 1.5f;
        [SerializeField] private float intimacyDecayRate  = 0.5f;
        [SerializeField] private float happinessDecayRate = 0.3f;

        [Header("상호작용 증가량")]
        [SerializeField] private float feedAmount    = 25f;
        [SerializeField] private float playAmount    = 20f;
        [SerializeField] private float touchAmount   = 5f;  // OnPetTouched 용

        // ─── 현재 수치 (읽기 전용 공개) ──────────────────────────
        public float Hunger    { get; private set; }
        public float Intimacy  { get; private set; }
        public float Happiness { get; private set; }

        // ─── UI 연동 이벤트 (SGMG) ────────────────────
        public event Action<float> OnHungerChanged;
        public event Action<float> OnIntimacyChanged;
        public event Action<float> OnHappinessChanged;

        // ─── Animator 연동 (KKB Animator 연결) ───────
        [Header("Animator 연결")]
        [SerializeField] private Animator petAnimator;

        // Animator Parameter 해시
        private static readonly int P_HUNGER    = Animator.StringToHash("HungerLevel");
        private static readonly int P_INTIMACY  = Animator.StringToHash("IntimacyLevel");
        private static readonly int P_HAPPINESS = Animator.StringToHash("HappinessLevel");
        private static readonly int P_IS_HUNGRY = Animator.StringToHash("IsHungry");
        private static readonly int P_IS_HAPPY  = Animator.StringToHash("IsHappy");
        private static readonly int P_EAT       = Animator.StringToHash("Eat");
        private static readonly int P_PLAY      = Animator.StringToHash("Play");

        // 임계치
        [Header("애니메이션 전환 임계치")]
        [SerializeField] private float hungryThreshold = 30f;
        [SerializeField] private float happyThreshold  = 70f;

        // Coroutine 캐싱
        private WaitForSeconds _decayTick;

        // ─────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Hunger    = initialHunger;
            Intimacy  = initialIntimacy;
            Happiness = initialHappiness;

            _decayTick = new WaitForSeconds(1f); // GC 절약
            StartCoroutine(DecayLoop());

            // MyPetManager의 빈 메서드와 연동
            // OnPetTouched → 친밀도 상승
        }

        private void OnEnable()  => SyncAnimator();
        private void OnDisable() => StopAllCoroutines();

        // ─────────────────────────────────────────────────────────
        //  Decay Loop (1초 주기)
        // ─────────────────────────────────────────────────────────

        private IEnumerator DecayLoop()
        {
            while (true)
            {
                yield return _decayTick;

                // 배고플수록 행복도 추가 감소 (패널티)
                float happinessPenalty = Hunger < hungryThreshold ? happinessDecayRate * 2f
                                                                   : happinessDecayRate;

                ApplyDecay(ref _hunger,    hungerDecayRate,    OnHungerChanged,    h => Hunger    = h);
                ApplyDecay(ref _intimacy,  intimacyDecayRate,  OnIntimacyChanged,  v => Intimacy  = v);
                ApplyDecay(ref _happiness, happinessPenalty,   OnHappinessChanged, v => Happiness = v);

                SyncAnimator();

                // MyPetManager에 수치 전달
                MyPetManager.myPetInstance?.ReportHealthChanged(Hunger);
            }
        }

        // 내부 backing field (ApplyDecay에서 ref 사용)
        private float _hunger;
        private float _intimacy;
        private float _happiness;

        private void ApplyDecay(ref float field, float rate,
                                Action<float> ev, Action<float> syncProp)
        {
            float prev = field;
            field = Mathf.Clamp(field - rate, 0f, 100f);
            syncProp(field);
            if (!Mathf.Approximately(prev, field))
                ev?.Invoke(field);
        }

        private void SetValue(ref float field, float newVal,
                              Action<float> ev, Action<float> syncProp)
        {
            field = Mathf.Clamp(newVal, 0f, 100f);
            syncProp(field);
            ev?.Invoke(field);
        }

        // ─────────────────────────────────────────────────────────
        //  공개 인터랙션 메서드
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// 먹이 주기 버튼 onClick에 연결
        /// 포만감을 상승시키고 Eat 애니메이션을 트리거
        /// </summary>
        public void Feed()
        {
            SetValue(ref _hunger, Hunger + feedAmount, OnHungerChanged, v => Hunger = v);
            petAnimator?.SetTrigger(P_EAT);
            SyncAnimator();
        }

        /// <summary>
        /// 놀아주기 버튼 onClick에 연결
        /// 친밀도와 행복도를 상승시키고 Play 애니메이션을 트리거
        /// </summary>
        public void Play()
        {
            SetValue(ref _intimacy,  Intimacy  + playAmount,       OnIntimacyChanged,  v => Intimacy  = v);
            SetValue(ref _happiness, Happiness + playAmount * 0.5f, OnHappinessChanged, v => Happiness = v);
            petAnimator?.SetTrigger(P_PLAY);
            SyncAnimator();
        }

        /// <summary>
        /// MyPetManager.OnPetTouched()에서 호출
        /// 캐릭터를 터치했을 때 친밀도를 소량 상승
        /// </summary>
        public void OnTouched()
        {
            SetValue(ref _intimacy, Intimacy + touchAmount, OnIntimacyChanged, v => Intimacy = v);
            SyncAnimator();
        }

        // ─────────────────────────────────────────────────────────
        //  Animator 동기화
        // ─────────────────────────────────────────────────────────

        private void SyncAnimator()
        {
            if (petAnimator == null) return;

            petAnimator.SetFloat(P_HUNGER,    Hunger);
            petAnimator.SetFloat(P_INTIMACY,  Intimacy);
            petAnimator.SetFloat(P_HAPPINESS, Happiness);
            petAnimator.SetBool(P_IS_HUNGRY,  Hunger    < hungryThreshold);
            petAnimator.SetBool(P_IS_HAPPY,   Happiness > happyThreshold);
        }
    }
}