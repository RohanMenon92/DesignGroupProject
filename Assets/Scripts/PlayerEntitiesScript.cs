using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntitiesScript : MonoBehaviour
{
    public Light stageLight;
    public List<Transform> stageEntities;
    public Transform cameraTransform;

    int maxTurn;
    // Start is called before the first frame update
    void Start()
    {
        maxTurn = stageEntities.Count;
        stageLight.intensity = 0;
        cameraTransform.DOLocalMove(EncounterConstants.startStageCamPos, 1f).SetEase(Ease.InOutBack);
        cameraTransform.DORotate(EncounterConstants.startStageCamRot, 1f).SetEase(Ease.InOutBack);
        stageLight.transform.DORotate(EncounterConstants.startLightRot, 1f);
        stageLight.DOIntensity(EncounterConstants.endLightIntensity, 1f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetBandEndTurn()
    {
        ChangeLightSpotAngle(90f);
        stageLight.DOIntensity(EncounterConstants.endLightIntensity, 1f);
        stageLight.transform.DORotate(EncounterConstants.startLightRot, 1f);
        stageLight.color = Color.white;

        cameraTransform.DOLocalMove(EncounterConstants.startStageCamPos, 1f).SetEase(Ease.InBack);
        cameraTransform.DORotate(EncounterConstants.startStageCamRot, 1f).SetEase(Ease.InOutBack);
    }

    public void TransitionToPlayer(int currentTurn)
    {
        ChangeLightSpotAngle(30f);
        Color spotLightColor = EncounterConstants.PlayerColors[currentTurn];

        Sequence transitionSequence = DOTween.Sequence();

        transitionSequence.Insert(0f, stageLight.DOColor(spotLightColor, 0.5f));
        transitionSequence.Insert(0f, stageLight.transform.DOLookAt(stageEntities[currentTurn].position, 1f).SetEase(Ease.InOutBack));

        Vector3 normalizedOrientation = -stageEntities[currentTurn].localPosition.normalized * 3;

        transitionSequence.Insert(0f, cameraTransform.DOMove(stageEntities[currentTurn].position +
            new Vector3(normalizedOrientation.x, 3, normalizedOrientation.z), 1f).SetEase(Ease.OutBack));
        transitionSequence.Insert(1f, cameraTransform.DOLookAt(stageEntities[currentTurn].position, 0.4f).SetEase(Ease.OutBack));
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
