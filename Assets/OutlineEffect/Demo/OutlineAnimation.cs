using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace cakeslice
{
    public class OutlineAnimation : MonoBehaviour
    {
        bool pingPong = false;

        OutlineEffect myOutlineEffect;

        // Use this for initialization
        void Start()
        {
            myOutlineEffect = GetComponent<OutlineEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            Color c = myOutlineEffect.lineColor0;

            if(pingPong)
            {
                c.a += Time.deltaTime;

                if(c.a >= 1)
                    pingPong = false;
            }
            else
            {
                c.a -= Time.deltaTime;

                if(c.a <= 0)
                    pingPong = true;
            }

            c.a = Mathf.Clamp01(c.a);
            myOutlineEffect.lineColor0 = c;
            myOutlineEffect.UpdateMaterialsPublicProperties();
        }
    }
}