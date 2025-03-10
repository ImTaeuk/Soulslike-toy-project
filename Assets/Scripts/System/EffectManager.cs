using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField] private GameObject swordHitEffectPref;
    [SerializeField] private GameObject countableEffectPref;


    private List<ParticleSystem> swordHitEffectPool;
    private List<ParticleSystem> countableEffectPool;

    private void Awake()
    {
        instance = this;

        swordHitEffectPool = new List<ParticleSystem>();
        countableEffectPool = new List<ParticleSystem>();

        for (int i = 0; i < 10; i++)
        {
            ParticleSystem inst;

            inst = Instantiate(swordHitEffectPref).GetComponent<ParticleSystem>();
            inst.gameObject.SetActive(false);
            swordHitEffectPool.Add(inst);

            inst = Instantiate(countableEffectPref).GetComponent<ParticleSystem>();
            inst.gameObject.SetActive(false);
            countableEffectPool.Add(inst);
        }
    }

    public ParticleSystem PlayCountableEffect(Transform parent, float durtaion)
    {
        for (int i = 0; i < countableEffectPool.Count; i++)
        {
            if (countableEffectPool[i].gameObject.activeSelf == false)
            {
                var m_module = countableEffectPool[i].main;

                m_module.duration = durtaion;
                m_module.startLifetime = durtaion;

                countableEffectPool[i].gameObject.SetActive(true);
                countableEffectPool[i].Play();

                countableEffectPool[i].transform.parent = parent;
                countableEffectPool[i].transform.localPosition = Vector3.zero;

                return countableEffectPool[i];
            }    
        }

        ParticleSystem inst = Instantiate(countableEffectPref).GetComponent<ParticleSystem>();

        inst.gameObject.SetActive(true);
        inst.Play();

        countableEffectPool.Add(inst);

        inst.transform.parent = parent;
        inst.transform.localPosition = Vector3.zero;

        var module = inst.main;

        module.duration = durtaion;
        module.startLifetime = durtaion;

        return inst;
    }
}
