using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitiesScript : MonoBehaviour
{
    public Light stageLight;
    public List<Transform> stageEntities;

    int maxTurn;
    // Start is called before the first frame update
    void Start()
    {
        maxTurn = stageEntities.Count;
        stageLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetBandEndTurn()
    {
        ChangeLightSpotAngle(90f);
        stageLight.DOIntensity(EncounterConstants.endLightIntensity, 1f);
        stageLight.transform.DORotate(new Vector3(90, 0, 0), 1f);
        stageLight.color = Color.white;
    }

    public void TransitionToPlayer(int currentTurn)
    {
        if(currentTurn == 0)
        {
            ChangeLightSpotAngle(30f);
        }
        Color spotLightColor = EncounterConstants.PlayerColors[currentTurn];

        stageLight.DOColor(spotLightColor, 0.5f);
        stageLight.transform.DOLookAt(stageEntities[currentTurn].position, 1f).SetEase(Ease.InOutBack);
    }

    public void ChangeLightSpotAngle(float value)
    {
        DOTween.To(() => stageLight.spotAngle, x => stageLight.spotAngle = x, value, 1f);
    }

    public void StartTurn()
    {
        stageLight.DOIntensity(EncounterConstants.startLightIntensity, 0.5f);
    }
}
