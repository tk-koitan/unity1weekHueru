using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoitanLib
{
    public class AnimationStressTest : MonoBehaviour
    {
        Camera camera;
        Vector3 mousePos;

        int cnt = 0;

        [SerializeField]
        private GameObject[] targets;
        [SerializeField]
        private GameObject lit;

        private List<GameObject> list = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            camera = Camera.main;
            Debug.Display(() => "TestTanCnt:" + cnt, this);
            Debug.Display(() => "GC:" + System.GC.CollectionCount(0), this);
        }

        // Update is called once per frame
        void Update()
        {
            mousePos = Input.mousePosition;
            mousePos.z = 10f;
            if (Input.GetMouseButtonDown(0))
            {
                GameObject target = targets[cnt % targets.Length];
                GameObject obj = Instantiate(target, camera.ScreenToWorldPoint(mousePos), Quaternion.identity);
                obj.SetActive(true);
                list.Add(obj);
                UnityEngine.Rendering.SortingGroup sg = obj.GetComponent<UnityEngine.Rendering.SortingGroup>();
                sg.sortingOrder = cnt * 100;
                cnt++;
            }
            if (Input.GetMouseButtonDown(1))
            {
                GameObject target = lit;
                GameObject obj = Instantiate(target, camera.ScreenToWorldPoint(mousePos), Quaternion.identity);
                obj.SetActive(true);
                list.Add(obj);
                cnt++;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (GameObject obj in list)
                {
                    Destroy(obj);
                }
                list.Clear();
                cnt = 0;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                System.GC.Collect();
            }
        }
    }
}


