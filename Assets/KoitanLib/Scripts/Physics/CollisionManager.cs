using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KoitanLib
{
    public class CollisionManager : MonoBehaviour
    {
        public static CollisionManager Instance;
        private int cnt = 0;
        private List<CollisionAABB> collisions = new List<CollisionAABB>();
        [SerializeField]
        private int initNum = 256;
        [SerializeField]
        CollisionAABB original;
        //1024個まで(適当)
        private static List<AABB> AABBList = new List<AABB>(1024);
        private static byte[] axisCount = new byte[523776];
        private static LinkedList<AABBPoint> xAxisAABB = new LinkedList<AABBPoint>();
        private static LinkedList<AABBPoint> yAxisAABB = new LinkedList<AABBPoint>();
        //スイーププルーンの走査の一時保存用
        private static LinkedList<int> sIdList = new LinkedList<int>();
        //ID振り分け用
        private static int idHead = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Display(() => "CollisionCount:" + cnt, this);
            //Debug.Display(() => "axisCount:" + string.Join(", ", axisCount), this);
            for (int i = 0; i < initNum; i++)
            {
                CollisionAABB col = Instantiate(original, new Vector3(Random.Range(-9f, 9f), Random.Range(-5f, 5f)), Quaternion.identity);
                col.size = new Vector2(Random.Range(0.5f, 1), Random.Range(0.5f, 1));
                col.transform.localScale = col.size;
                col.UpdateAABB();
                collisions.Add(col);
            }
            original.gameObject.SetActive(false);
            Init();
        }

        private void Update()
        {
            //追加
            if (Input.GetKeyDown(KeyCode.A))
            {
                CollisionAABB col = Instantiate(original, new Vector3(Random.Range(-9f, 9f), Random.Range(-5f, 5f)), Quaternion.identity);
                col.gameObject.SetActive(true);
                col.size = new Vector2(Random.Range(0.5f, 1), Random.Range(0.5f, 1));
                col.transform.localScale = col.size;
                col.UpdateAABB();
                collisions.Add(col);
                AddAABB(new AABB(idHead, col.Min, col.Max));
                idHead++;
            }

            //最適化なし
            cnt = 0;
            /*
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    bool result = list[i].IsOverlap(list[j]);
                    if (result)
                    {
                        //Debug.DrawLine2D(list[i].Center(), list[j].Center(), 0.1f, Color.red);
                        UnityEngine.Debug.DrawLine(list[i].Center(), list[j].Center());
                    }
                    cnt++;
                }
            }
            */
            //移動
            for (int i = 0; i < collisions.Count; i++)
            {
                CollisionAABB c = collisions[i];
                AABBList[i].ChangePos(c.Center());
            }

            //スイーププルーン
            //初期化
            int max = AABBList.Count * (AABBList.Count - 1) / 2;
            for (int i = 0; i < max; i++)
            {
                axisCount[i] = 0;
            }
            //x軸
            for (LinkedListNode<AABBPoint> node = xAxisAABB.First; node != null; node = node.Next)
            {
                AABBPoint point = node.Value;
                //終点
                if (point.isEnd)
                {
                    //始点リストから自分を削除する、AABBがポインタを持ってるのでO(1)
                    sIdList.Remove(AABBList[point.rootId].xId);

                    //デバッグ用
                    if (sIdList.Count == 0)
                    {
                        Debug.DrawLine2D(new Vector3(point.pos, -10), new Vector3(point.pos, 10), 0.05f, new Color(0, 0, 1, 0.5f));
                    }
                }
                //始点
                else
                {
                    //始点リストとの衝突回数を増やす
                    for (LinkedListNode<int> tmpNode = sIdList.First; tmpNode != null; tmpNode = tmpNode.Next)
                    {
                        int tmpId = tmpNode.Value;
                        axisCount[ListIndex(point.rootId, tmpId)]++;
                    }
                    //最後に自身を追加する
                    sIdList.AddLast(AABBList[point.rootId].xId);
                }
            }
            //y軸
            for (LinkedListNode<AABBPoint> node = yAxisAABB.First; node != null; node = node.Next)
            {
                AABBPoint point = node.Value;
                //終点
                if (point.isEnd)
                {
                    //始点リストから自分を削除する、AABBがポインタを持ってるのでO(1)
                    sIdList.Remove(AABBList[point.rootId].yId);

                    //デバッグ用
                    if (sIdList.Count == 0)
                    {
                        Debug.DrawLine2D(new Vector3(-10, point.pos), new Vector3(10, point.pos), 0.05f, new Color(0, 0, 1, 0.5f));
                    }
                }
                //始点
                else
                {
                    //始点リストとの衝突回数を増やす
                    for (LinkedListNode<int> tmpNode = sIdList.First; tmpNode != null; tmpNode = tmpNode.Next)
                    {
                        int tmpId = tmpNode.Value;
                        axisCount[ListIndex(point.rootId, tmpId)]++;
                        if (axisCount[ListIndex(point.rootId, tmpId)] >= 2)
                        {
                            //UnityEngine.Debug.DrawLine(AABBList[point.rootId].Center(), AABBList[tmpId].Center(), Color.white);
                            //Debug.DrawLine2D(AABBList[point.rootId].Center(), AABBList[tmpId].Center(), 0.1f, Color.red);
                            Debug.DrawArrow2D(AABBList[point.rootId].Center(), AABBList[tmpId].Center(), 0.05f, new Color(1, 0, 0, 0.5f));
                            //Debug.Log("衝突ペア(" + point.rootId + ", " + tmpId + ")");
                        }
                    }
                    //最後に自身を追加する
                    sIdList.AddLast(AABBList[point.rootId].yId);
                }
            }
        }

        private void Init()
        {
            //id振る
            List<LinkedListNode<AABBPoint>> xTmpList = new List<LinkedListNode<AABBPoint>>();
            List<LinkedListNode<AABBPoint>> yTmpList = new List<LinkedListNode<AABBPoint>>();
            for (int i = 0; i < collisions.Count; i++)
            {
                CollisionAABB c = collisions[i];
                c.ID = i;
                AABBList.Add(new AABB(i, c.Min, c.Max));
                xTmpList.Add(AABBList[i].xMin);
                xTmpList.Add(AABBList[i].xMax);
                yTmpList.Add(AABBList[i].yMin);
                yTmpList.Add(AABBList[i].yMax);
            }
            //ソートする
            xTmpList.Sort((a, b) => a.Value.pos.CompareTo(b.Value.pos));
            yTmpList.Sort((a, b) => a.Value.pos.CompareTo(b.Value.pos));
            //連結リストに入れる
            for (int i = 0; i < collisions.Count * 2; i++)
            {
                xAxisAABB.AddLast(xTmpList[i]);
                yAxisAABB.AddLast(yTmpList[i]);
            }
            idHead = collisions.Count;
        }

        public void AddAABB(AABB aabb)
        {
            AABBList.Add(aabb);
            AABBPoint xMin = aabb.xMin.Value;
            AABBPoint xMax = aabb.xMax.Value;
            //x軸
            for (LinkedListNode<AABBPoint> node = xAxisAABB.First; node != null; node = node.Next)
            {
                AABBPoint point = node.Value;
                if (xMin.pos < point.pos)
                {
                    xAxisAABB.AddBefore(node, aabb.xMin);
                    for (LinkedListNode<AABBPoint> node2 = node; node2 != null; node2 = node2.Next)
                    {
                        AABBPoint point2 = node2.Value;
                        if (xMax.pos < point2.pos)
                        {
                            xAxisAABB.AddBefore(node2, aabb.xMax);
                            break;
                        }
                        else if (node2.Next == null)
                        {
                            xAxisAABB.AddLast(aabb.xMax);
                            break;
                        }
                    }
                    break;
                }
                else if (node.Next == null)
                {
                    xAxisAABB.AddLast(aabb.xMin);
                    xAxisAABB.AddLast(aabb.xMax);
                    break;
                }
            }
            AABBPoint yMin = aabb.yMin.Value;
            AABBPoint yMax = aabb.yMax.Value;
            //y軸
            for (LinkedListNode<AABBPoint> node = yAxisAABB.First; node != null; node = node.Next)
            {
                AABBPoint point = node.Value;
                if (yMin.pos < point.pos)
                {
                    yAxisAABB.AddBefore(node, aabb.yMin);
                    for (LinkedListNode<AABBPoint> node2 = node; node2 != null; node2 = node2.Next)
                    {
                        AABBPoint point2 = node2.Value;
                        if (yMax.pos < point2.pos)
                        {
                            yAxisAABB.AddBefore(node2, aabb.yMax);
                            break;
                        }
                        else if (node2.Next == null)
                        {
                            yAxisAABB.AddLast(aabb.yMax);
                            break;
                        }
                    }
                    break;
                }
                else if (node.Next == null)
                {
                    yAxisAABB.AddLast(aabb.yMin);
                    yAxisAABB.AddLast(aabb.yMax);
                    break;
                }
            }
        }


        //a>bで[0,n*(n-1)/2)に単射
        //a<bのときは入れ替えてくれる
        private int ListIndex(int a, int b)
        {
            if (a < b)
            {
                int tmp = b;
                b = a;
                a = tmp;
            }
            return (a - 2) * (a + 1) / 2 + 1 + b;
        }

        public class AABBPoint
        {
            public int rootId;
            public bool isEnd;
            public float pos;
            public AABBPoint(int rootId, bool isEnd, float pos)
            {
                this.rootId = rootId;
                this.isEnd = isEnd;
                this.pos = pos;
            }
        }

        public class AABB
        {
            public int id;
            public Vector2 min;
            public Vector2 max;
            public float xPos;
            public float yPos;
            public float width;
            public float height;
            public LinkedListNode<AABBPoint> xMin;
            public LinkedListNode<AABBPoint> xMax;
            public LinkedListNode<int> xId;
            public LinkedListNode<AABBPoint> yMin;
            public LinkedListNode<AABBPoint> yMax;
            public LinkedListNode<int> yId;

            public AABB(int id, float xPos, float yPos, float width, float height)
            {
                this.id = id;
                this.xPos = xPos;
                this.yPos = yPos;
                this.width = width;
                this.height = height;
                min = new Vector2(xPos - width / 2, yPos - height / 2);
                max = new Vector2(xPos + width / 2, yPos + height / 2);
                xMin = new LinkedListNode<AABBPoint>(new AABBPoint(id, false, min.x));
                xMax = new LinkedListNode<AABBPoint>(new AABBPoint(id, true, max.x));
                xId = new LinkedListNode<int>(id);
                yMin = new LinkedListNode<AABBPoint>(new AABBPoint(id, false, min.y));
                yMax = new LinkedListNode<AABBPoint>(new AABBPoint(id, true, max.y));
                yId = new LinkedListNode<int>(id);
            }

            public AABB(int id, Vector2 min, Vector2 max)
            {
                this.id = id;
                this.min = min;
                this.max = max;
                xPos = (min.x + max.y) / 2;
                yPos = (min.y + max.y) / 2;
                width = max.x - min.x;
                height = max.y - min.y;
                xMin = new LinkedListNode<AABBPoint>(new AABBPoint(id, false, min.x));
                xMax = new LinkedListNode<AABBPoint>(new AABBPoint(id, true, max.x));
                xId = new LinkedListNode<int>(id);
                yMin = new LinkedListNode<AABBPoint>(new AABBPoint(id, false, min.y));
                yMax = new LinkedListNode<AABBPoint>(new AABBPoint(id, true, max.y));
                yId = new LinkedListNode<int>(id);
            }

            public Vector2 Center()
            {
                return new Vector2(xPos, yPos);
            }

            public void ChangePos(Vector2 pos)
            {
                ChangePosX(pos.x);
                ChangePosY(pos.y);
            }

            public void ChangePosX(float newX)
            {
                //左に移動
                if (newX < xPos)
                {
                    xPos = newX;
                    min.x = newX - width / 2;
                    max.x = newX + width / 2;
                    xMin.Value.pos = min.x;
                    xMax.Value.pos = max.x;
                    //もう少し最適化できる
                    for (LinkedListNode<AABBPoint> node = xMin.Previous; node != null; node = node.Previous)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos < min.x)
                        {
                            xAxisAABB.Remove(xMin);
                            xAxisAABB.AddAfter(node, xMin);
                            break;
                        }
                        else if (node.Previous == null)
                        {
                            xAxisAABB.Remove(xMin);
                            xAxisAABB.AddBefore(node, xMin);
                            break;
                        }
                    }
                    for (LinkedListNode<AABBPoint> node = xMax.Previous; node != null; node = node.Previous)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos < max.x)
                        {
                            xAxisAABB.Remove(xMax);
                            xAxisAABB.AddAfter(node, xMax);
                            break;
                        }
                        else if (node.Previous == null)
                        {
                            xAxisAABB.Remove(xMax);
                            xAxisAABB.AddBefore(node, xMax);
                            break;
                        }
                    }
                }
                //右に移動
                else if (newX > xPos)
                {
                    xPos = newX;
                    min.x = newX - width / 2;
                    max.x = newX + width / 2;
                    xMin.Value.pos = min.x;
                    xMax.Value.pos = max.x;
                    for (LinkedListNode<AABBPoint> node = xMax.Next; node != null; node = node.Next)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos > max.x)
                        {
                            xAxisAABB.Remove(xMax);
                            xAxisAABB.AddBefore(node, xMax);
                            break;
                        }
                        else if (node.Next == null)
                        {
                            xAxisAABB.Remove(xMax);
                            xAxisAABB.AddAfter(node, xMax);
                            break;
                        }
                    }
                    for (LinkedListNode<AABBPoint> node = xMin.Next; node != null; node = node.Next)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos > min.x)
                        {
                            xAxisAABB.Remove(xMin);
                            xAxisAABB.AddBefore(node, xMin);
                            break;
                        }
                        else if (node.Next == null)
                        {
                            xAxisAABB.Remove(xMin);
                            xAxisAABB.AddAfter(node, xMin);
                            break;
                        }
                    }
                }
            }

            public void ChangePosY(float newY)
            {
                //下に移動
                if (newY < yPos)
                {
                    yPos = newY;
                    min.y = newY - width / 2;
                    max.y = newY + width / 2;
                    yMin.Value.pos = min.y;
                    yMax.Value.pos = max.y;
                    //もう少し最適化できる
                    for (LinkedListNode<AABBPoint> node = yMin.Previous; node != null; node = node.Previous)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos < min.y)
                        {
                            yAxisAABB.Remove(yMin);
                            yAxisAABB.AddAfter(node, yMin);
                            break;
                        }
                        else if (node.Previous == null)
                        {
                            yAxisAABB.Remove(yMin);
                            yAxisAABB.AddBefore(node, yMin);
                            break;
                        }
                    }
                    for (LinkedListNode<AABBPoint> node = yMax.Previous; node != null; node = node.Previous)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos < max.y)
                        {
                            yAxisAABB.Remove(yMax);
                            yAxisAABB.AddAfter(node, yMax);
                            break;
                        }
                        else if (node.Previous == null)
                        {
                            yAxisAABB.Remove(yMax);
                            yAxisAABB.AddBefore(node, yMax);
                            break;
                        }
                    }
                }
                //上に移動
                else if (newY > yPos)
                {
                    yPos = newY;
                    min.y = newY - width / 2;
                    max.y = newY + width / 2;
                    yMin.Value.pos = min.y;
                    yMax.Value.pos = max.y;
                    for (LinkedListNode<AABBPoint> node = yMax.Next; node != null; node = node.Next)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos > max.y)
                        {
                            yAxisAABB.Remove(yMax);
                            yAxisAABB.AddBefore(node, yMax);
                            break;
                        }
                        else if (node.Next == null)
                        {
                            yAxisAABB.Remove(yMax);
                            yAxisAABB.AddAfter(node, yMax);
                            break;
                        }
                    }
                    for (LinkedListNode<AABBPoint> node = yMin.Next; node != null; node = node.Next)
                    {
                        AABBPoint point = node.Value;
                        if (point.pos > min.y)
                        {
                            yAxisAABB.Remove(yMin);
                            yAxisAABB.AddBefore(node, yMin);
                            break;
                        }
                        else if (node.Next == null)
                        {
                            yAxisAABB.Remove(yMin);
                            yAxisAABB.AddAfter(node, yMin);
                            break;
                        }
                    }
                }
            }
        }
    }
}