using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoitanLib
{
    public class CollisionAABB : MonoBehaviour
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }
        public Vector2 size = new Vector2(2, 2);
        public Vector2 offset = new Vector2();
        public Color color = Color.white;
        public int ID;
        public Vector2 velocity;
        // Start is called before the first frame update
        void Start()
        {
            if((float)Random.Range(0,int.MaxValue)/int.MaxValue < 0.5f)
            {
                velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            }
            else
            {
                velocity = new Vector2();
            }

            //size

            UpdateAABB();
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(velocity * Time.deltaTime);
            if (velocity.x > 0 && transform.position.x > 9f) velocity.x = -velocity.x;
            if (velocity.x < 0 && transform.position.x < -9f) velocity.x = -velocity.x;
            if (velocity.y > 0 && transform.position.y > 5f) velocity.y = -velocity.y;
            if (velocity.y < 0 && transform.position.y < -5f) velocity.y = -velocity.y;
            UpdateAABB();
            //Debug.DrawRect2D(size.x, size.y, (Vector2)transform.position + offset, color);
            Debug.ShowText(ID.ToString(), transform.position);
        }

        public void UpdateAABB()
        {
            Min = (Vector2)transform.position + offset - size / 2;
            Max = (Vector2)transform.position + offset + size / 2;
            //CollisionManager.Instance.ChangeAABB(ID);
        }

        //AABBが重なっていたらtrue
        public bool IsOverlap(CollisionAABB other)
        {
            if (other.Min.x < this.Max.x && this.Min.x < other.Max.x && other.Min.y < this.Max.y && this.Min.y < other.Max.y)
            {
                this.color = Color.red;
                other.color = Color.red;
                return true;
            }
            else
            {
                this.color = Color.white;
                other.color = Color.white;
                return false;
            }            
        }

        public Vector2 Center()
        {
            return (Min + Max) / 2;
        }        
    }
}