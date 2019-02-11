using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public Image BombButton = null;
    public Sprite EmptyBomb = null;
    public Sprite FillBomb = null;
    public Text BombText = null;
    public Object BombAnimationObject = null;
    public GameObject Panel = null;

    private SoundManager mSoundManager = null;

    public float BombCount
    {
        get; private set;
    }

    public void OnClickBomb()
    {
        if(BombCount > 0)
        {
            mSoundManager.PlayOneShot("bomb-explosion");

            BombCount -= 1;

            // 폭탄 애니메이션을 생성하고, 이 애니메이션을 0.25 초 뒤에 삭제한다
            GameObject obj = Instantiate(BombAnimationObject, Panel.transform) as GameObject;
            obj.transform.SetParent(Panel.transform);
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
                        zombie.TakeDamage(zombie.LifeMax);
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

        mSoundManager = SoundManager.Instance;
    }

    void Update()
    {
        BombText.text = "X " + BombCount.ToString();
        // 폭탄의 개수에 따라 폭탄 버튼의 리소스를 결정한다
        if (BombCount > 0)
        {
            BombButton.sprite = FillBomb;
        }
        else
        {
            BombButton.sprite = EmptyBomb;
        }
    }
}
