using UnityEngine;
using UnityEngine.UI;

public class Menus : MonoBehaviour {

    [SerializeField] private GameObject lightStore;
    [SerializeField] private GameObject lightMaps;
    [SerializeField] private GameObject lightItems;
    [SerializeField] private GameObject lightMessages;
    [SerializeField] private GameObject lightRanking;
    [SerializeField] private GameObject lightSettings;
    [SerializeField] private GameObject lightMission;
    [SerializeField] private GameObject lightAchievements;
    [SerializeField] private GameObject lightLevelSelect;
    [SerializeField] private GameObject lightRewards;

    [Space(10), SerializeField]
     private GameObject darkStore;
    [SerializeField] private GameObject darkMaps;
    [SerializeField] private GameObject darkItems;
    [SerializeField] private GameObject darkMessages;
    [SerializeField] private GameObject darkRanking;
    [SerializeField] private GameObject darkSettings;
    [SerializeField] private GameObject darkMission;
    [SerializeField] private GameObject darkAchievements;
    [SerializeField] private GameObject darkLevelSelect;
    [SerializeField] private GameObject darkRewards;

    [Space(10), SerializeField]
     private GameObject lightLogin;
    [SerializeField] private GameObject darkLogin;
    [SerializeField] private GameObject DemoSelect;
    [SerializeField] private GameObject LightTheme;
    [SerializeField] private GameObject DarkTheme;

    [Space(10), SerializeField]
     private Button lightStoreButton;
    [SerializeField] private Button lightMapsButton;
    [SerializeField] private Button lightItemsButton;
    [SerializeField] private Button lightMessagesButton;
    [SerializeField] private Button lightRankingButton;
    [SerializeField] private Button lightSettingsButton;
    [SerializeField] private Button lightMissionButton;
    [SerializeField] private Button lightLoginButton;
    [SerializeField] private Button lightAchievementsButton;
    [SerializeField] private Button lightLevelSelectButton;
    [SerializeField] private Button lightRewardsButton;

    [Space(10), SerializeField]
     private Button DarkButton;
    [SerializeField] private Button LightButton;

    [Space(10), SerializeField]
     private Button darkStoreButton;
    [SerializeField] private Button darkMapsButton;
    [SerializeField] private Button darkItemsButton;
    [SerializeField] private Button darkMessagesButton;
    [SerializeField] private Button darkRankingButton;
    [SerializeField] private Button darkSettingsButton;
    [SerializeField] private Button darkMissionButton;
    [SerializeField] private Button darkLoginButton;
    [SerializeField] private Button darkAchievementsButton;
    [SerializeField] private Button darkLevelSelectButton;
    [SerializeField] private Button darkRewardsButton;

    [Space(10), SerializeField]
     private Button BackToDemoSelectButton;


    [Space(10)]
    public GameObject MenuDark;
    public GameObject MenuLight;
    public string themeColor;


    public LoadingScript loadingScript;
    public StartScript startScript;

    [Space(10)]
    public GameObject backButton;

    [SerializeField] private GameObject Panel;
    private void Start() {

        this.DarkButton.onClick.AddListener(this.OpenDarkTheme);
        this.LightButton.onClick.AddListener(this.OpenLightTheme);

        this.lightStoreButton.onClick.AddListener(this.LightStore);
        this.lightMapsButton.onClick.AddListener(this.LightMaps);
        this.lightItemsButton.onClick.AddListener(this.LightItems);
        this.lightMessagesButton.onClick.AddListener(this.LightMessages);
        this.lightRankingButton.onClick.AddListener(this.LightRanking);
        this.lightSettingsButton.onClick.AddListener(this.LightSettings);
        this.lightMissionButton.onClick.AddListener(this.LightMission);
        this.lightLevelSelectButton.onClick.AddListener(this.LightLevelSelect);
        this.lightAchievementsButton.onClick.AddListener(this.LightAchievements);
        this.lightRewardsButton.onClick.AddListener(this.LightDailyRewards);

        this.darkStoreButton.onClick.AddListener(this.DarkStore);
        this.darkMapsButton.onClick.AddListener(this.DarkMaps);
        this.darkItemsButton.onClick.AddListener(this.DarkItems);
        this.darkMessagesButton.onClick.AddListener(this.DarkMessages);
        this.darkRankingButton.onClick.AddListener(this.DarkRanking);
        this.darkSettingsButton.onClick.AddListener(this.DarkSettings);
        this.darkMissionButton.onClick.AddListener(this.DarkMission);
        this.darkLevelSelectButton.onClick.AddListener(this.DarkLevelSelect);
        this.darkAchievementsButton.onClick.AddListener(this.DarkAchievements);
        this.darkRewardsButton.onClick.AddListener(this.darkDailyRewards);

        this.lightLoginButton.onClick.AddListener(this.LightLogin);
        this.darkLoginButton.onClick.AddListener(this.DarkLogin);

        this.BackToDemoSelectButton.onClick.AddListener(this.BackButton);
        this.backButton.SetActive(false);
    }

    private void LightLogin() {
        this.Panel.SetActive(true);
        this.lightLogin.SetActive(true);
    }

    private void DarkLogin() {
        this.Panel.SetActive(true);
        this.darkLogin.SetActive(true);
    }

    private void OpenDarkTheme() {
        this.themeColor = "Dark";
        this.DemoSelect.SetActive(false);
        this.MenuDark.SetActive(true);
        this.startScript.gemsAndCoin.SetActive(true);
        this.backButton.SetActive(true);
    }

    private void OpenLightTheme() {
        this.themeColor = "Light";
        this.DemoSelect.SetActive(false);
        this.MenuLight.SetActive(true);
        this.startScript.gemsAndCoin.SetActive(true);
        this.backButton.SetActive(true);
    }

    private void LightDailyRewards() {
        this.Panel.SetActive(true);
        this.lightRewards.SetActive(true);
    }
    private void LightStore() {
        this.Panel.SetActive(true);
        this.lightStore.SetActive(true);
    }

    private void LightMaps() {
        this.Panel.SetActive(true);
        this.lightMaps.SetActive(true);
        Debug.Log(" Maps");
    }

    private void LightItems() {
        this.Panel.SetActive(true);
        this.lightItems.SetActive(true);
        Debug.Log(" Items");
    }

    private void LightMessages() {
        this.Panel.SetActive(true);
        this.lightMessages.SetActive(true);
        Debug.Log(" Messages");
    }

    private void LightRanking() {
        this.Panel.SetActive(true);
        this.lightRanking.SetActive(true);
        Debug.Log(" Ranking");
    }

    private void LightSettings() {
        this.Panel.SetActive(true);
        this.lightSettings.SetActive(true);
        Debug.Log(" Settings");
    }

    private void LightMission() {
        this.Panel.SetActive(true);
        this.lightMission.SetActive(true);
        Debug.Log(" Mission");
    }

    private void LightLevelSelect() {
        this.Panel.SetActive(true);
        this.lightLevelSelect.SetActive(true);
    }

    private void LightAchievements() {
        this.Panel.SetActive(true);
        this.lightAchievements.SetActive(true);
    }

    //---------------------------------------------------------------------------------------------
    //-------------------------------DARK THEME---------------------------------------------------------
    private void darkDailyRewards() {
        this.Panel.SetActive(true);
        this.darkRewards.SetActive(true);
    }
    private void DarkAchievements() {
        this.Panel.SetActive(true);
        this.darkAchievements.SetActive(true);
    }




    private void DarkLevelSelect() {
        this.Panel.SetActive(true);
        this.darkLevelSelect.SetActive(true);
    }


    private void DarkStore() {
        this.Panel.SetActive(true);
        this.darkStore.SetActive(true);
        Debug.Log(" Store");
    }

    private void DarkMaps() {
        this.Panel.SetActive(true);
        this.darkMaps.SetActive(true);
        Debug.Log(" Maps");
    }

    private void DarkItems() {
        this.Panel.SetActive(true);
        this.darkItems.SetActive(true);
        Debug.Log(" Items");
    }

    private void DarkMessages() {
        this.Panel.SetActive(true);
        this.darkMessages.SetActive(true);
        Debug.Log(" Messages");
    }

    private void DarkRanking() {
        this.Panel.SetActive(true);
        this.darkRanking.SetActive(true);
        Debug.Log(" Ranking");
    }

    private void DarkSettings() {
        this.Panel.SetActive(true);
        this.darkSettings.SetActive(true);
        Debug.Log(" Settings");
    }

    private void DarkMission() {
        this.Panel.SetActive(true);
        this.darkMission.SetActive(true);
        Debug.Log(" Mission");
    }

    private void BackButton() {
        this.loadingScript.loadingProgress.value = 0;
        this.loadingScript.loadValue = 0.0f;
        this.themeColor = " ";
        this.DemoSelect.SetActive(true);
        this.MenuLight.SetActive(false);
        this.MenuDark.SetActive(false);
        this.startScript.gemsAndCoin.SetActive(false);
        this.startScript.startingPage.SetActive(false);
        this.backButton.SetActive(false);
    }
}
