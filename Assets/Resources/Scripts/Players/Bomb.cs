using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public EventManager eventManager = null;

    public Button bombButton = null;
    public Image bombButtonImage = null;
    public Sprite emptyBomb = null;
    public Sprite fillBomb = null;
    public Text bombText = null;
    public Object bombAnimationObject = null;
    public GameObject panel = null;

    private Fever mFever = null;
    private SoundManager mSoundManager = null;

    public float BombCount
    {
        get; private set;
    }

    public void BlockBomb()
    {
        bombButton.onClick.RemoveAllListeners();
    }

    public void GrantBomb()
    {
        bombButton.onClick.AddListener(OnClickBomb);
    }

    public void OnClickBomb()
    {
        if ((BombCount > 0) && (mFever.IsFeverOn == false))
        {
            mSoundManager.PlayOneShot("bomb-explosion");

            BombCount -= 1;

            // 폭탄 애니메이션을 생성하고, 이 애니메이션을 0.25 초 뒤에 삭제한다
            GameObject obj = Instantiate(bombAnimationObject, panel.transform) as GameObject;
            obj.transform.SetParent(panel.transform);
            Destroy(obj, 0.25f);
            // 좀비를 가지고 있는 target 을 순회하면서, 각 좀비를 죽인다
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (GameObject target in targets)
            {
                if (target.transform.childCount > 0)
                {
                    Zombie zombie = target.transform.GetChild(0).GetComponent<Zombie>();

                    if (zombie != null)
                    {
                        zombie.TakeDamage(zombie.lifeMax);
                    }
                }
            }
        }
    }

    public void AddBomb()
    {
        mSoundManager.PlayOneShot("bomb-get");

        BombCount += 1;
    }

    void Awake()
    {
        BombCount = 3;
        // 게임이 pause 되었을 때 removeListener 가 정상적으로 작동할 수 있도록 하기 위해
        // 여기에서 OnClick 을 할당
        bombButton.onClick.AddListener(OnClickBomb);

        mFever = eventManager.fever;
    }

    void Start()
    {
        mSoundManager = SoundManager.Instance;
    }

    void Update()
    {
        bombText.text = "X " + BombCount.ToString();
        // 폭탄의 개수에 따라 폭탄 버튼의 리소스를 결정한다
        if (BombCount > 0)
        {
            bombButtonImage.sprite = fillBomb;
        }
        else
        {
            bombButtonImage.sprite = emptyBomb;
        }
    }
}
