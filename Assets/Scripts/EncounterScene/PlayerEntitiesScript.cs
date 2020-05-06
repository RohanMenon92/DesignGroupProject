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

    EncounterConstants encounterConstants;

    // Start is called before the first frame update
    void Awake()
    {
        // Take values from encounter constants
        encounterConstants = FindObjectOfType<EncounterConstants>();
    }
    void Start()
    {
        maxTurn = stageEntities.Count;
        stageLight.intensity = 0;
        stageLight.spotAngle = 0;
        cameraTransform.DOLocalMove(encounterConstants.stageCamStartPos, 1f).SetEase(Ease.InOutBack);
        cameraTransform.DORotate(encounterConstants.stageCamStartRot, 1f).SetEase(Ease.InOutBack);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnGameStart()
    {
        ChangeLightSpotAngle(encounterConstants.spotLightGeneral);
        stageLight.transform.DORotate(encounterConstants.startLightRot, 1f);
        stageLight.DOIntensity(encounterConstants.startLightEntity, 1f);
    }

    public void OnHyped()
    {
        ChangeLightSpotAngle(encounterConstants.spotLightGeneral);
        stageLight.DOColor(encounterConstants.HypeLight, 0.5f);
        stageLight.transform.DORotate(encounterConstants.startLightRot, 0.5f);
        stageLight.DOIntensity(encounterConstants.startLightEntity, 1f);

        cameraTransform.DOLocalMove(encounterConstants.stageCamStartPos, 1f).SetEase(Ease.InBack);
        cameraTransform.DORotate(encounterConstants.stageCamStartRot, 1f).SetEase(Ease.InOutBack);
    }

    public void ResetBandEndTurn()
    {
        ChangeLightSpotAngle(0f);

        stageLight.DOIntensity(encounterConstants.endLightEntity, 1f);
        stageLight.transform.DORotate(encounterConstants.startLightRot, 1f);
        stageLight.color = Color.white;

        cameraTransform.DOLocalMove(encounterConstants.stageCamStartPos, 1f).SetEase(Ease.InBack);
        cameraTransform.DORotate(encounterConstants.stageCamStartRot, 1f).SetEase(Ease.InOutBack);
    }

    public void TransitionToPlayer(int currentTurn)
    {
        ChangeLightSpotAngle(encounterConstants.spotLightFocus);
        Color spotLightColor = encounterConstants.PlayerColors[currentTurn];

        Sequence transitionSequence = DOTween.Sequence();

        transitionSequence.Insert(0f, stageLight.DOColor(spotLightColor, 0.5f));
        transitionSequence.Insert(0f, stageLight.transform.DOLookAt(stageEntities[currentTurn].position, 1f).SetEase(Ease.InOutBack));

        transitionSequence.Insert(0f, stageLight.DOIntensity(encounterConstants.focusLightEntity, 1f));
        Vector3 normalizedOrientation = -stageEntities[currentTurn].localPosition.normalized * 3;

        transitionSequence.Insert(0f, cameraTransform.DOMove(stageEntities[currentTurn].position +
            new Vector3(normalizedOrientation.x, encounterConstants.stageCameraOffsetY, normalizedOrientation.z), 1f).SetEase(Ease.OutBack));
        transitionSequence.Insert(1f, cameraTransform.DOLookAt(stageEntities[currentTurn].position, 0.4f).SetEase(Ease.OutBack));
    }

    public void ChangeLightSpotAngle(float value)
    {
        DOTween.To(() => stageLight.spotAngle, x => stageLight.spotAngle = x, value, 1f).SetEase(Ease.InOutBack);
    }

    public void StartTurn()
    {
        stageLight.DOIntensity(encounterConstants.startLightEntity, 0.5f);
    }
}
