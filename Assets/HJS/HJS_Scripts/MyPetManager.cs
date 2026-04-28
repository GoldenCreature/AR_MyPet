using System;
using UnityEngine;
using CMS.AR_MyPet; // �μ��� �鿣�� ���� : PetStatusController
using SGMG.AR_MyPet; // �αԴ� UI : HungerBar, MoodBar, HappinessBar

namespace HJS.AR_MyPet
{
    /// <summary>
    /// AR �� ������Ʈ�� ��ü �帧�� �����ϴ� �߾� ��Ʈ�� Ÿ��
    /// ������ UI ������ �긴�� ������ ����
    /// </summary>
    public class MyPetManager : MonoBehaviour
    {
        // [�̱���] �ܺο��� �б⸸ ������ �ν��Ͻ�
        public static MyPetManager myPetInstance { get; private set; }

        [Header("���� Ȯ��")]
        [Tooltip("���� ���� ���� �����Ǿ� ��ϵǾ����� ����")]
        public bool isPetSpawned { get; private set; } = false;

        // �� ���� ��Ʈ�ѷ� ������ ����
        private PetStatusController status;

        [Header("�ν����� �Ҵ�")]
        [SerializeField, Tooltip("��ȯ�� �� ������Ʈ�� ������")]
        private GameObject currentPet;

        [SerializeField, Tooltip("������ ��ġ�� �ð�ȭ�ϴ� �����̴� ��ũ��Ʈ")]
        private HungerBar hungerBarUI;

        [SerializeField, Tooltip("�ູ�� ��ġ�� �ð�ȭ�ϴ� �����̴� ��ũ��Ʈ")]
        private HappinessBar happinessBarUI;

        [SerializeField, Tooltip("ģ�е�(���) ��ġ�� �ð�ȭ�ϴ� �����̴� ��ũ��Ʈ")]
        private MoodBar moodBarUI;

        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // ����� �� �ε庸�� ���� �ڵ� ����
        static void InitManager()
        {
            // ���̾��Ű�� �� �޴����� �ִ��� Ȯ��
            GameObject myPetManager = GameObject.Find("myPetManager");
            if (myPetManager == null)
            {
                // ������ ������Ʈ ���� �� ������Ʈ �߰�
                myPetManager = new GameObject("myPetManager");
                myPetManager.AddComponent<MyPetManager>();
                myPetManager.transform.position = Vector3.zero;
                Debug.Log("<color=cyan>MyPetManager:</color> �Ŵ��� �ڵ� ���� �Ϸ�");
            }
        }
        

        // [��ü ������]
        private void Awake() 
        {
            // [�ߺ� ����] �̹� �Ŵ����� �ִٸ� ���� ����� �Ŵ����� ���� 
            if(myPetInstance != null && myPetInstance != this)
            {
                Destroy(this.gameObject);
                return;
            } 
            
            myPetInstance = this; 
            DontDestroyOnLoad(this.gameObject); // �� ��ȯ �ÿ��� ����
        }

        /// <summary>
        /// AR �ٴ� ��ġ �� ��ȯ�� ���� �޴����� ����ϴ� �Լ�
        /// </summary>
        /// <param name="pet">������ ���� GameObject �����͸� �����Ͻÿ�</param>
        public void RegisterPet(GameObject pet)
        {
            if (pet == null) return;
            
            PetStatusController newStatus = pet.GetComponent<PetStatusController>();
            if (status == null)
            {
                Debug.LogWarning("<color=red>MyPetManager: PetStatusController�� ã�� �� �����ϴ�!");
                return;
            }
            else
                Debug.Log("<color=green>MyPetManager:</color> PetStatusController ����");

            status = newStatus;
            currentPet = pet;
            isPetSpawned = true;
            Debug.Log("<color=green>MyPetManager:</color> ��Ŵ����� ���� ��� �Ϸ�, isPetSpawned => true ");

            // ���� ��ġ�� ���� ������ �αԴ� UI �����̴��� �����ϵ��� �̺�Ʈ ����
            BindEvents();

            RefreshAllUI(); // ����� ���� ���� UI�� ����ȭ
            Debug.Log("<color=green>MyPetManager:</color> UI ����ȭ �Ϸ�");
        }

        /// <summary>
        /// ����Ƽ ���� ���� �̺�Ʈ �Լ�
        /// </summary>
        private void BindEvents()
        {
            if (status == null) return;

            // �ߺ� ������ ���� ���� ����ص� ���ٽ� ���� 
            if (hungerBarUI != null) status.OnHungerChanged -= UpdateHungerUI;
            if (happinessBarUI != null) status.OnHappinessChanged -= UpdateHappinessUI;
            if (moodBarUI != null) status.OnIntimacyChanged -= UpdateMoodUI;

            // ���� ��� 
            if (hungerBarUI != null) status.OnHungerChanged += UpdateHungerUI;
            if (happinessBarUI != null) status.OnHappinessChanged += UpdateHappinessUI;
            if (moodBarUI != null) status.OnIntimacyChanged += UpdateMoodUI;
        }

        private void UpdateHungerUI(float val) { hungerBarUI.hungerSlider.value = val; }
        private void UpdateHappinessUI(float val) { happinessBarUI.happinessSlider.value = val; }
        private void UpdateMoodUI(float val) { moodBarUI.moodSlider.value = val; }

        
        /// <summary>
        /// �� ������ ���� Ȯ��
        /// </summary>
        /// <param name="hungerValue">�ٲ� ����� ��ġ�� �Է��Ͻÿ�</param>
        public void ReportHungerChanged(float hungerValue)
        {
            if(!isPetSpawned) return;
            Debug.Log($"�� ����� ��ġ {hungerValue}�� ����");
        }

        /// <summary>
        /// ��� UI �����̴��� ���� ���� ������ ��ġ�� ��ġȭ
        /// </summary>
        public void RefreshAllUI()
        {
            if (status == null) return;

            if (hungerBarUI != null)
            {
                hungerBarUI.hungerSlider.value = status.Hunger;
            }
            if (moodBarUI != null)
            {
                moodBarUI.moodSlider.value = status.Intimacy;
            }
            if (happinessBarUI != null)
            {
                happinessBarUI.happinessSlider.value = status.Happiness;
            }
        }

        /// <summary>
        /// �����ֱ� ��ư �������� ȣ��
        /// </summary>
        public void OnFeedButtonClicked()
        {
            // ���� ��ȯ���� �ʾҰų� ������ ������ ����
            if (!isPetSpawned || status == null) return;

            Debug.Log("<color=yellow>MyPetManager:</color> �����ֱ� ��� ����");
            status?.Feed(); 
        }

        /// <summary>
        /// ����ֱ� ��ư�� ������ �� ȣ�� 
        /// </summary>
        public void OnPlayButtonClicked()
        {
            if (!isPetSpawned || status == null) return;

            Debug.Log("<color=yellow>MyPetManager:</color> ����ֱ� ��� ����");
            status?.Play(); 
        }

        /// <summary>
        /// �� ��ġ �̺�Ʈ �߻� �� �������ͽ� ��Ʈ�ѷ��� ����
        /// </summary>
        public void OnPetTouched()
        {
            if (!isPetSpawned) return;

            Debug.Log("���� ��ġ�Ǿ����ϴ�.");
            status?.OnTouched();

        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            // [����] �μ��� ���� ��Ʈ�ѷ� �ּ� ��������
            //status = PetStatusController.Instance;
            //if(status == null) 
            //{
            //    Debug.LogWarning("<color=red>MyPetManager: PetStatusController�� ã�� �� �����ϴ�!");
            //    return; 
            //}

            if(hungerBarUI == null) hungerBarUI = FindFirstObjectByType<HungerBar>();
            if(happinessBarUI == null) happinessBarUI = FindFirstObjectByType<HappinessBar>();
            if(moodBarUI == null) moodBarUI = FindFirstObjectByType<MoodBar>();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void OnDestroy()
        {
            if (status != null)
            {
                status.OnHungerChanged -= UpdateHungerUI;
                status.OnHappinessChanged -= UpdateHappinessUI;
                status.OnIntimacyChanged -= UpdateMoodUI;
                Debug.Log("<color=yellow>MyPetManager:</color> �̺�Ʈ ���� ���� �� �޸� ���� �Ϸ�");
            }
        }
    }

}
