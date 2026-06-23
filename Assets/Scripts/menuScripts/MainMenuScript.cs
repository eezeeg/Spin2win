using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public static bool isLevelSelect;
    public static int selectedPage = 0;

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

    [SerializeField] private float accessoriesCanvasMoveDur = 1f;
    [SerializeField] private float accessoriesHiddenX = -1920f;
    [SerializeField] private float accessoriesShownX = 0f;
    private Coroutine accessoriesCanvasCoroutine;
    [Header("button prefab")]
    [SerializeField] private Button buttonPrefab; 

    [Header("Second Camera")]
    [SerializeField] private Camera SecondCam;
    [SerializeField] private float camMoveDur = 1f;

    private Coroutine viewportCoroutine;

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

    private void Awake()
    {
        Vector2 pos = accessoriesCanvas.anchoredPosition;
        pos.x = accessoriesHiddenX;
        accessoriesCanvas.anchoredPosition = pos;
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

            newButton.onClick.AddListener(() =>
            {
                SelectShape(currentShape);
            });
        }
    }
    private void SelectMaterial(Material material)
    {
        Renderer renderer = marble.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }
    private void SelectMaterial2(Material material)
    {
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
    public void showAccPanel(int panelInt)
    {
        switch (panelInt)
        {
            case 1:
                panel1.SetActive(true);
                panel2.SetActive(false);
                panel3.SetActive(false);
                break;
            case 2:
                panel1.SetActive(false);
                panel2.SetActive(true);
                panel3.SetActive(false);
                break;
            case 3:
                panel1.SetActive(false);
                panel2.SetActive(false);
                panel3.SetActive(true);
                break;
        }
    }
    public void GoAccesories(bool isGoing)
    {
        if (viewportCoroutine != null)
        {
            StopCoroutine(viewportCoroutine);
        }

        if (accessoriesCanvasCoroutine != null)
        {
            StopCoroutine(accessoriesCanvasCoroutine);
        }


        if (isGoing)
        {
            viewportCoroutine = StartCoroutine(LerpCameraViewport(SecondCam, 0f)); 
            accessoriesCanvasCoroutine = StartCoroutine(LerpCanvas(accessoriesCanvas, accessoriesShownX));
        }
        else
        {
            viewportCoroutine = StartCoroutine(LerpCameraViewport(SecondCam, 0.3f)); 
            accessoriesCanvasCoroutine = StartCoroutine(LerpCanvas(accessoriesCanvas, accessoriesHiddenX));
        }
    }

    private IEnumerator LerpCameraViewport(Camera cam, float targetx)
    {
        Rect startRect = cam.rect;
        Rect targetRect = cam.rect;
        targetRect.x = targetx;
        float elapsed = 0f;

        while (elapsed < camMoveDur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / camMoveDur;
            t = Mathf.SmoothStep(0f, 1f, t);

            Rect newRect = cam.rect;
            newRect.x = Mathf.Lerp(startRect.x, targetRect.x, t);
            cam.rect = newRect;

            yield return null;
        }

        cam.rect = targetRect;
        viewportCoroutine = null;
    }

    private IEnumerator LerpCanvas(RectTransform canvasRect, float targetX)
    {
        Vector2 startPos = canvasRect.anchoredPosition;
        Vector2 targetPos = startPos;
        targetPos.x = targetX;

        float elapsed = 0f;
        while (elapsed < accessoriesCanvasMoveDur)
        {
            elapsed += Time.deltaTime;

            float time = elapsed / accessoriesCanvasMoveDur;
            time = Mathf.SmoothStep(0f, 1f, time);

            canvasRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, time);

            yield return null;
        }
        canvasRect.anchoredPosition = targetPos;
        accessoriesCanvasCoroutine = null;
    }
    public void QuitGame()
    {
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