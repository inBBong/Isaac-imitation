using UnityEngine;
enum ItemNum //18개의 아이템 넘버
{
    CricketsHead=0,
    BloodoftheMartyr,
    GrowthHormones,
    RoidRage,
    LordofthePit,
    Fate,
    SpeedBall,
    WireCoatHanger,
    MomsUnderwear,
    MomsHeels,
    MomsLipstick,
    Technology,
    OuijaBoard,
    TwentyTwenty,
    SpoonBender,
    MomsKnife,
    Stemcell,
    SuperBandage
}
public class Item : MonoBehaviour
{
    float animOffset = 0f;
    float t=0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Animation();
    }

    void Animation()
    {
        transform.Translate(new Vector3(0, animOffset*Time.deltaTime));
        t += Time.deltaTime*3.0f;
        animOffset = Mathf.Sin(t)*0.15f;
    }
}

