using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite easySpr, medSpr, hardSpr, endSpr;
    [SerializeField] private Sprite ace, king, queen;

    public bool isGameActive;

    public GameObject titleScreen;
    public GameObject gameMenu;
    public GameObject finalMenu;
    public GameObject WindowWebView;
    public GameObject AlphaObj;
    //private Image AlphaImage;
    //public Color color = Color.white;
    public bool isFlash;

    [SerializeField] private int _point;
    private float _timeLeft;
    private int difLvl;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;

    public TextMeshProUGUI tripletsText;
    private int _triplets;
    public TextMeshProUGUI totalPairsText;
    private int _totalPairs;

    public TextMeshProUGUI triesText;
    private int _tries;

    public TextMeshProUGUI totalTriesText;
    private int _totalTries;

    public TextMeshProUGUI scoreText;
    private int _score;

    public TextMeshProUGUI totalScoreText;
    private int _totalScore;

    private float _luck;
    public TextMeshProUGUI luckText;
    private float _totalLuck;
    public TextMeshProUGUI totalLuckText;

    [SerializeField] private int _allCards;

    private int _gridRows; //столбцы
    private int _gridCols; //строки
    [SerializeField] private float _offsetX;
    [SerializeField] private float _offsetY;
    [SerializeField] private CardPrefab cardPrefab;
    [SerializeField] private Sprite[] imagesPrefab;
    private CardPrefab _firstRevealed;
    private CardPrefab _secondRevealed;
    private CardPrefab _thirdRevealed;

    public bool CanReveal => _thirdRevealed == null;

    public Button[] difButtons;
    public Sprite star, noStar;
    public Button restartButton;
    public Button homeButton;
    public Button exitButton;

    public Image finalImage;

    [SerializeField] private string _starsKeyPlayerPrefs;
    [SerializeField] private string _scoresKeyPlayerPrefs;

    public int[] NumArray { get; private set; }

    private void Awake()
    {
        Load();
        titleScreen.gameObject.SetActive(true);
        cardPrefab.gameObject.SetActive(false);
        gameMenu.gameObject.SetActive(false);
        finalMenu.gameObject.SetActive(false);
    }
    private void Start()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (PlayerPrefs.HasKey(_starsKeyPlayerPrefs + i))
            {
                if (PlayerPrefs.GetInt(_starsKeyPlayerPrefs + i) == 1)
                {
                    difButtons[i - 1].transform.GetChild(0).GetComponent<Image>().sprite = star;
                    difButtons[i - 1].transform.GetChild(1).GetComponent<Image>().sprite = noStar;
                    difButtons[i - 1].transform.GetChild(2).GetComponent<Image>().sprite = noStar;
                }
                else if (PlayerPrefs.GetInt(_starsKeyPlayerPrefs + i) == 2)
                {
                    difButtons[i - 1].transform.GetChild(0).GetComponent<Image>().sprite = star;
                    difButtons[i - 1].transform.GetChild(1).GetComponent<Image>().sprite = star;
                    difButtons[i - 1].transform.GetChild(2).GetComponent<Image>().sprite = noStar;
                }
                else if (PlayerPrefs.GetInt(_starsKeyPlayerPrefs + i) == 3)
                {
                    difButtons[i - 1].transform.GetChild(0).GetComponent<Image>().sprite = star;
                    difButtons[i - 1].transform.GetChild(1).GetComponent<Image>().sprite = star;
                    difButtons[i - 1].transform.GetChild(2).GetComponent<Image>().sprite = star;
                }
            }
            else
            {
                difButtons[i - 1].transform.GetChild(0).gameObject.SetActive(false);
                difButtons[i - 1].transform.GetChild(1).gameObject.SetActive(false);
                difButtons[i - 1].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }
    void Update()
    {
        if (isGameActive)
        {
            _timeLeft -= Time.deltaTime;
            timerText.SetText("TIME: " + Mathf.Round(_timeLeft));
            if (_timeLeft < 0 || (_allCards / (difLvl / 3.5)) <= _tries)
            {
                GameOver();
            }
            else if (_triplets == (_allCards / 3))
            {
                Win();
            }
        }
    }
    public void StartGame(int difficulty)
    {
        difLvl = difficulty;
        _point = 1;
        titleScreen.gameObject.SetActive(false);
        //Debug.Log("difLvl " + difLvl);
        _timeLeft = 240;
        //Debug.Log("_timeLeft " + _timeLeft);
        _timeLeft /= difficulty;
        //Debug.Log(" _timeLeft /= difficulty " + _timeLeft);

        if (difLvl == 1)
        {
            _allCards = 12;
            restartButton.GetComponent<Image>().sprite = easySpr;
        }
        else if (difLvl == 2)
        {
            _allCards = 15;
            restartButton.GetComponent<Image>().sprite = medSpr;
        }
        else
        {
            _allCards = 24;
            restartButton.GetComponent<Image>().sprite = hardSpr;
        }

        AllCardPrefabs();

        _triplets = 0;
        _tries = 0;
        StartCoroutine(DealPref());
        //StartCoroutine(SkipBackAllCards());
        isGameActive = true;
    }
    private void AllCardPrefabs()
    {
        if (_allCards % 3 == 0)
        {
            if (_allCards == 12)
            {
                _gridCols = 4;
                _gridRows = 3;
            }
            else if (_allCards == 15)
            {
                _gridCols = 3;
                _gridRows = 5;
            }
            else if (_allCards == 24)
            {
                _gridCols = 4;
                _gridRows = 6;
            }
            else
            {
                _gridCols = 4;
                _gridRows = _allCards / _gridCols;
            }
        }
        else
        {
            _allCards = 12;
            AllCardPrefabs();
        }
    }
    //Debug.Log("");
    private IEnumerator DealPref()
    {
        if (_gridCols % 3 == 0)
        {
            cardPrefab.transform.Translate(_offsetX / 2, 0, 0);
        }
        cardPrefab.gameObject.SetActive(true);
        gameMenu.gameObject.SetActive(true);

        Vector3 startPos = cardPrefab.transform.position;

        if (difLvl == 1)
        {
            int[] numbers = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 }; //12
            NumArray = ShuffleArray(numbers);
        }
        else if (difLvl == 2)
        {
            int[] numbers = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4 }; //15
            NumArray = ShuffleArray(numbers);
        }
        else
        {
            int[] numbers = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7 }; //24
            NumArray = ShuffleArray(numbers);
        }

        //for (int i = 0; i < _gridCols; i++)
        for (int j = 0; j < _gridRows; j++)
        {
            //for (int j = 0; j < _gridRows; j++)
            for (int i = 0; i < _gridCols; i++)
            {
                CardPrefab clonePrefab;
                if (i == 0 && j == 0)
                {
                    clonePrefab = cardPrefab;
                }
                else
                {
                    clonePrefab = Instantiate(cardPrefab) as CardPrefab;
                }
                int index = j * _gridCols + i;
                int id = NumArray[index];
                clonePrefab.SetCard(id, imagesPrefab[id]);

                float posX = (_offsetX * i) + startPos.x;
                float posY = -(_offsetY * j) + startPos.y;
                
                clonePrefab.transform.position = new Vector3(posX, posY + (_offsetY * j + _offsetY), startPos.z);
                yield return new WaitForSeconds(0.05f);
                clonePrefab.transform.position = new Vector3(posX, posY + (_offsetY * j) / 2, startPos.z);
                yield return new WaitForSeconds(0.05f);
                clonePrefab.transform.position = new Vector3(posX, posY + (_offsetY * j) / 4, startPos.z);
                yield return new WaitForSeconds(0.05f);
                clonePrefab.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
        StartCoroutine(SkipBackAllCards());
    }
    public IEnumerator SkipBackAllCards()
    {
        GameObject[] skipBackAllC;
        skipBackAllC = GameObject.FindGameObjectsWithTag("Card");
        if (skipBackAllC.Length == 0)
        {
            Debug.Log("No game objects are tagged with 'Card'");
        }
        else
        {
            for (int i = 0; i < skipBackAllC.Length; i++)
            {
                skipBackAllC[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(1.5f * difLvl);
            for (int i = 0; i < skipBackAllC.Length; i++)
            {
                skipBackAllC[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int temp = newArray[i];
            int random = Random.Range(i, newArray.Length);
            newArray[i] = newArray[random];
            newArray[random] = temp;
        }
        return newArray;
    }

    public void CardRevealed(CardPrefab cardPrefab)
    {
        if (_firstRevealed == null)
        {
            _firstRevealed = cardPrefab;
        }
        else if (_firstRevealed != null && _secondRevealed == null)
        {
            _secondRevealed = cardPrefab;
            StartCoroutine(CheckTriplet());
        }
        else
        {
            _thirdRevealed = cardPrefab;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckTriplet()
    {

        //AlphaImage = AlphaObj.GetComponent<Image>();

        if (_firstRevealed.Id != _secondRevealed.Id)
        {
            StartCoroutine(FlashAndSkipAllCards());
            yield return new WaitForSeconds(.5f);

            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();

            _firstRevealed = null;
            _secondRevealed = null;

            _tries++;
            triesText.text = "TRIES: " + _tries;
        }
    }

    public IEnumerator FlashAndSkipAllCards()
    {
        GameObject[] changeAlphaColor;
        changeAlphaColor = GameObject.FindGameObjectsWithTag("Card");
        if (changeAlphaColor.Length == 0)
        {
            Debug.Log("No game objects are tagged with 'Card'");
        }
        else
        {
            isFlash = true;
            AndroidNativeCore.Vibrator.Vibrate(100);
            AlphaObj.SetActive(true);
            for (int i = 0; i < changeAlphaColor.Length; i++)
            {
                changeAlphaColor[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < changeAlphaColor.Length; i++)
            {
                //AlphaObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
                changeAlphaColor[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i < changeAlphaColor.Length; i++)
            {
                //AlphaObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                changeAlphaColor[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.75f);
            }
            yield return new WaitForSeconds(0.15f);
            for (int i = 0; i < changeAlphaColor.Length; i++)
            {
                //AlphaObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
                changeAlphaColor[i].transform.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                isFlash = false;
                AlphaObj.SetActive(false);
            }
        }

    }
    private IEnumerator CheckMatch()
    {
        _tries++;
        triesText.text = "TRIES: " + _tries;

        if (_firstRevealed.Id == _secondRevealed.Id && _firstRevealed.Id == _thirdRevealed.Id)
        {
            _triplets++;
            tripletsText.text = "PAIRS: " + _triplets;

            _score = (int)(_triplets * _timeLeft * difLvl * _point);
            scoreText.text = "SCORE: " + _score;
            //Debug.Log("score " + _score);
            _score++;
        }
        else
        {
            StartCoroutine(FlashAndSkipAllCards());
            yield return new WaitForSeconds(.5f);
            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
            _thirdRevealed.Unreveal();
        }

        _luck = _triplets / (_tries * 1f);
        //Debug.Log("Luck " + (_luck * 100) + " %");

        _firstRevealed = null;
        _secondRevealed = null;
        _thirdRevealed = null;
    }

    private void Scoring()
    {
        _totalTries += _tries;
        _totalPairs += _triplets;
        _totalScore += _score;
        _totalLuck = _totalPairs / (_totalTries * 1f);
        Save();
    }
    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        isGameActive = false;
        restartButton.GetComponent<Image>().sprite = endSpr;

        Scoring();

        luckText.text = "Your Luck: " + Mathf.Round(_luck * 100) + " %";
    }
    public void Win()
    {
        isGameActive = false;

        KillAllPrefabs();

        gameMenu.gameObject.SetActive(false);
        finalMenu.gameObject.SetActive(true);

        Scoring();

        totalTriesText.text = "Total TRIES: " + _totalTries;
        totalPairsText.text = "Total PAIRS: " + _totalPairs;
        totalScoreText.text = "Total SCORE: " + _totalScore;
        totalLuckText.text = "Total LUCK: " + Mathf.Round(_totalLuck * 100) + " %";
        luckText.text = "Your Luck: " + Mathf.Round(_luck * 100) + " %";

        if ((_luck < 0.75f) && !PlayerPrefs.HasKey(_starsKeyPlayerPrefs + difLvl))
        {
            PlayerPrefs.SetInt(_starsKeyPlayerPrefs + difLvl, 1);
        }
        else if ((_luck >= 0.75f) && _luck <= 0.9f && (!PlayerPrefs.HasKey(_starsKeyPlayerPrefs + difLvl) || PlayerPrefs.GetInt(_starsKeyPlayerPrefs + difLvl) < 2))
        {
            PlayerPrefs.SetInt(_starsKeyPlayerPrefs + difLvl, 2);
        }
        else if ((_luck > 0.9f) && (!PlayerPrefs.HasKey(_starsKeyPlayerPrefs + difLvl) || PlayerPrefs.GetInt(_starsKeyPlayerPrefs + difLvl) < 3))
        {
            PlayerPrefs.SetInt(_starsKeyPlayerPrefs + difLvl, 3);
        }
        Debug.Log(PlayerPrefs.GetInt(_starsKeyPlayerPrefs + difLvl));

        if (_luck > 0.9f)
        {
            finalImage.sprite = ace;
        }
        else if (_luck >= 0.75f)
        {
            finalImage.sprite = king;
        }
        else
        {
            finalImage.sprite = queen;
        }
    }
    private void Load()
    {
        string key = _scoresKeyPlayerPrefs;
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);

            SaveData data = JsonUtility.FromJson<SaveData>(value);
            this._totalPairs = data.pairs;
            this._totalScore = data.scores;
            this._totalTries = data.tries;
            this._totalLuck = data.luck;
        }
    }
    public void Save()
    {
        string key = _scoresKeyPlayerPrefs;

        SaveData data = new SaveData();
        data.pairs = this._totalPairs;
        data.scores = this._totalScore;
        data.tries = this._totalTries;
        data.luck = this._totalLuck;

        string value = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, value);

        PlayerPrefs.Save();
    }
    public void KillAllPrefabs()
    {
        GameObject[] killEmAll;
        killEmAll = GameObject.FindGameObjectsWithTag("Card");
        if (killEmAll.Length == 0)
        {
            Debug.Log("No game objects are tagged with 'Card'");
        }
        else
        {
            for (int i = 0; i < killEmAll.Length; i++)
            {
                Destroy(killEmAll[i].gameObject);
            }
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Home()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        Application.Quit();
#endif
    }
}
