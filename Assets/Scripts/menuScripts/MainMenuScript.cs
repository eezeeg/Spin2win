using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public static bool isLevelSelect;
    public static int selectedPage = 0;

    public static Material selectedMat1;
    public static Material selectedMat2;
    public static int ShapeId;

    [Header("Testing")]
    public GameObject marble;
    public GameObject tieMarble;

    [Header("Accessories Canvas")]
    [SerializeField] private RectTransform accessoriesCanvas;
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;
    [SerializeField] private GameObject panel3;

    [Header("Prefabs accesoires")]
    [SerializeField] private List<Material> MatList1;
    [SerializeField] private List<Material> MatList2;
    [SerializeField] private List<GameObject> Shape1List;

    [Header("Button prefab")]
    [SerializeField] private Button buttonPrefab;

    [Header("Second Camera")]
    [SerializeField] private Camera SecondCam;
    [SerializeField] private float accessoriesCameraViewportX = 0.3f;
    [SerializeField] private float mainMenuCameraViewportX = 0.3f;

    [Header("Level select pages")]
    [SerializeField] private List<GameObject> levelPageList;

    [Header("Panels")]
    [SerializeField] private GameObject panelMain;

    [Header("Buttons")]
    [SerializeField] private List<GameObject> levelList;

    [Header("Camera")]
    [SerializeField] private GameObject CameraToHide;

    [Header("Star Sprites")]
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    [Header("Settings")]
    [SerializeField] private SettingsMenu settingsMenu;

    private void Awake()
    {
        if (accessoriesCanvas != null)
        {
            accessoriesCanvas.gameObject.SetActive(false);
        }

        SetSecondCameraViewportX(mainMenuCameraViewportX);

        CreateAccButtons();

        if (isLevelSelect)
        {
            PlayGame();
        }
    }

    private void Start()
    {
        RefreshMenu();
    }

    public void pageUp(bool isNext)
    {
        levelPageList[selectedPage].SetActive(false);
        selectedPage += isNext ? 1 : -1;
        levelPageList[selectedPage].SetActive(true);
    }

    public void PlayGame()
    {
        settingsMenu.CloseSettings();
        panelMain.SetActive(false);
        levelPageList[selectedPage].SetActive(true);
        CameraToHide.SetActive(false);
        isLevelSelect = true;
    }

    private void CreateAccButtons()
    {
        CreateMaterial1Buttons();
        CreateMaterial2Buttons();
        CreatShapeButton();

        LayoutRebuilder.ForceRebuildLayoutImmediate(panel1.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel2.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel3.GetComponent<RectTransform>());
    }

    private void CreateMaterial1Buttons()
    {
        Debug.Log("Creating material buttons. Count: " + MatList1.Count);

        foreach (Material material in MatList1)
        {
            Debug.Log("material: " + material.name);

            Button newButton = Instantiate(buttonPrefab, panel1.transform);
            newButton.gameObject.SetActive(true);
            newButton.name = material.name;

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = material.name;
            }

            Material currentMaterial = material;

            newButton.onClick.AddListener(() =>
            {
                SelectMaterial(currentMaterial);
            });
        }
    }

    private void CreateMaterial2Buttons()
    {
        Debug.Log("Creating material 2 buttons. Count: " + MatList2.Count);

        foreach (Material material in MatList2)
        {
            Debug.Log("material 2: " + material.name);

            Button newButton = Instantiate(buttonPrefab, panel3.transform);
            newButton.gameObject.SetActive(true);
            newButton.name = material.name;

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>(true);

            if (buttonText != null)
            {
                buttonText.text = material.name;
            }

            Material currentMaterial = material;

            newButton.onClick.AddListener(() =>
            {
                SelectMaterial2(currentMaterial);
            });
        }
    }

    private void CreatShapeButton()
    {
        Debug.Log("Creating shape buttons. Count: " + Shape1List.Count);

        int shapeId = 0;

        foreach (GameObject shape in Shape1List)
        {
            Debug.Log("shape: " + shape.name);

            Button newButton = Instantiate(buttonPrefab, panel2.transform);
            newButton.gameObject.SetActive(true);
            newButton.name = shape.name;

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>(true);

            if (buttonText != null)
            {
                buttonText.text = shape.name;
            }

            GameObject currentShape = shape;
            int currentShapeId = shapeId;

            newButton.onClick.AddListener(() =>
            {
                SelectShape(currentShape);
                ShapeId = currentShapeId;
            });

            shapeId++;
        }
    }

    private void SelectMaterial(Material material)
    {
        selectedMat1 = material;
        Renderer renderer = marble.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    private void SelectMaterial2(Material material)
    {
        selectedMat2 = material;
        Renderer renderer = tieMarble.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    private void SelectShape(GameObject shape)
    {
        MeshFilter marbleMeshFilter = tieMarble.GetComponent<MeshFilter>();
        MeshFilter shapeMeshFilter = shape.GetComponentInChildren<MeshFilter>();

        if (marbleMeshFilter == null)
        {
            return;
        }

        if (shapeMeshFilter == null)
        {
            return;
        }

        marbleMeshFilter.mesh = shapeMeshFilter.sharedMesh;
        tieMarble.transform.localScale = shape.transform.localScale;
    }

    public void GoAccesories(bool isGoing)
    {
        settingsMenu.CloseSettings();
        if (isGoing)
        {
            panelMain.SetActive(false);

            if (accessoriesCanvas != null)
            {
                accessoriesCanvas.gameObject.SetActive(true);
            }

            SetSecondCameraViewportX(accessoriesCameraViewportX);
        }
        else
        {
            if (accessoriesCanvas != null)
            {
                accessoriesCanvas.gameObject.SetActive(false);
            }

            panelMain.SetActive(true);
            SetSecondCameraViewportX(mainMenuCameraViewportX);
        }
    }

    private void SetSecondCameraViewportX(float x)
    {
        if (SecondCam == null)
        {
            return;
        }

        Rect rect = SecondCam.rect;
        rect.x = x;
        SecondCam.rect = rect;
    }

    public void QuitGame()
    {
        settingsMenu.CloseSettings();
        Application.Quit();
    }

    public void LoadLevel(int levelId)
    {
        string level = "Level_" + levelId;
        StartCoroutine(LoadLevel(level));
    }

    private IEnumerator LoadLevel(string levelName)
    {
        TransitionManager.Instance.LoadSceneWithFade(levelName);
        yield break;
    }

    public void ReturnMain()
    {
        panelMain.SetActive(true);
        levelPageList[selectedPage].SetActive(false);
        CameraToHide.SetActive(true);
        isLevelSelect = false;
    }

    private void RefreshMenu()
    {
        int totalComplete = 0;
        int levelId = 1;

        foreach (GameObject level in levelList)
        {
            TMP_Text timer = level.transform.Find("CompleteTime").GetComponent<TMP_Text>();
            Transform starsParent = level.transform.Find("LvlImage1/Stars");

            GameObject buttonObject = level.transform.Find("BtnLvl").gameObject;
            Button button = buttonObject.GetComponent<Button>();
            TMP_Text btnText = buttonObject.GetComponentInChildren<TMP_Text>();

            timer.text = "--:--.--";
            button.interactable = true;
            btnText.text = "Level " + levelId;

            SetLevelStars(starsParent, 0);

            if (LevelsBeatSave.IsLevelComplete(levelId))
            {
                totalComplete++;

                float time = LevelsBeatSave.GetBestTime(levelId);
                int stars = LevelsBeatSave.GetStars(levelId);

                timer.text = FormatTime(time);
                SetLevelStars(starsParent, stars);
            }

            if (levelId > 3)
            {
                if (totalComplete + 3 < levelId)
                {
                    button.interactable = false;
                    btnText.text = "locked";
                    SetLevelStars(starsParent, 0);
                }
            }

            levelId++;
        }

        CollectionStatsUI[] statsUIs = FindObjectsOfType<CollectionStatsUI>(true);

        foreach (CollectionStatsUI statsUI in statsUIs)
        {
            statsUI.Refresh();
        }
    }

    private void SetLevelStars(Transform starsParent, int starCount)
    {
        if (starsParent == null)
        {
            return;
        }

        starCount = Mathf.Clamp(starCount, 0, 3);

        for (int i = 0; i < starsParent.childCount; i++)
        {
            Image starImage = starsParent.GetChild(i).GetComponent<Image>();

            if (starImage == null)
            {
                continue;
            }

            starImage.sprite = i < starCount ? filledStar : emptyStar;
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100f);

        return minutes.ToString("00") + ":" +
               seconds.ToString("00") + "." +
               milliseconds.ToString("00");
    }

    public void DeleteData()
    {
        LevelsBeatSave.DeleteEVERYTHING();
        RefreshMenu();
    }
}