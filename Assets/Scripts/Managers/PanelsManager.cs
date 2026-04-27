using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager Instance;
    [Header("Panels")]
    [SerializeField] GameObject MusclePanel;
    [SerializeField] GameObject BonePanel;
    [SerializeField] GameObject Joints_OrgansPanel;

    [Header("Panels Secundare")]
    [SerializeField] GameObject EnemyPanel;
    [SerializeField] GameObject AdditionalInfoPanel;


    [Header("ToolTip")]
    [SerializeField] Tooltip ToolTip;

    private bool BoneShow;
    private bool MuscleShow;
    private bool Joints_OrgansShow;
    private bool EnemyShow;

    /// <summary>
    /// Initializeaza Singleton-ul si reseteaza starea tuturor panourilor la invizibil
    /// </summary>
    private void Awake()
    {
        Instance = this;
        MusclePanel.SetActive(false);
        BonePanel.SetActive(false);
        Joints_OrgansPanel.SetActive(false);
        EnemyPanel.SetActive(false);
        AdditionalInfoPanel.SetActive(false);
        BoneShow = false;
        MuscleShow = false;
        Joints_OrgansShow = false;
        EnemyShow = false;
    }

    /// <summary>
    /// Asculta input-ul de la tastatura pentru comutarea panourilor in faza de strategie
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.State == GameState.Strategy)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {

                ToggleBonePanel();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {

                ToggleMusclesPanel();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {

                ToggleJoints_OrgansPanel();
            }
        }
        else
        {
            CloseAdditionalInfoPanel();
        }

        if (BoneShow == false && MuscleShow == false && Joints_OrgansShow == false)
        {
            ToolTip.Toggle(false);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleEnemyPanel();
        }
    }

    /// <summary>
    /// Comuta vizibilitatea panoului de oase si inchide celelalte panouri anatomice active
    /// </summary>
    private void ToggleBonePanel()
    {
        if (MuscleShow)
        {
            MusclePanel.SetActive(false);
            MuscleShow = false;
            ToolTip.Toggle(false);
        }
        if (Joints_OrgansShow)
        {
            Joints_OrgansPanel.SetActive(false);
            Joints_OrgansShow = false;
            ToolTip.Toggle(false);
        }

        if (!BoneShow)
        {
            BonePanel.SetActive(true);
            BoneShow = true;
            CloseAdditionalInfoPanel();
            ToolTip.SetBoneSize();
        }
        else
        {
            BonePanel.SetActive(false);
            BoneShow = false;
        }
    }

    /// <summary>
    /// Comuta vizibilitatea panoului de muschi si inchide celelalte panouri anatomice active
    /// </summary>
    private void ToggleMusclesPanel()
    {
        if (BoneShow)
        {
            BonePanel.SetActive(false);
            BoneShow = false;
            ToolTip.Toggle(false);
        }
        if (Joints_OrgansShow)
        {
            Joints_OrgansPanel.SetActive(false);
            Joints_OrgansShow = false;
            ToolTip.Toggle(false);
        }

        if (!MuscleShow)
        {
            MusclePanel.SetActive(true);
            MuscleShow = true;
            CloseAdditionalInfoPanel();
            ToolTip.SetMuscleSize();
        }
        else
        {
            MusclePanel.SetActive(false);
            MuscleShow = false;
        }
    }

    /// <summary>
    /// Comuta vizibilitatea panoului de articulatii/organe si inchide celelalte panouri anatomice active
    /// </summary>
    private void ToggleJoints_OrgansPanel()
    {
        if (BoneShow)
        {
            BonePanel.SetActive(false);
            BoneShow = false;
            ToolTip.Toggle(false);
        }
        if (MuscleShow)
        {
            MusclePanel.SetActive(false);
            MuscleShow = false;
            ToolTip.Toggle(false);
        }

        if (!Joints_OrgansShow)
        {
            Joints_OrgansPanel.SetActive(true);
            Joints_OrgansShow = true;
            CloseAdditionalInfoPanel();
            ToolTip.SetJointOrganSize();
        }
        else
        {
            Joints_OrgansPanel.SetActive(false);
            Joints_OrgansShow = false;
        }
    }

    /// <summary>
    /// Comuta vizibilitatea panoului cu informatiile inamicului
    /// </summary>
    private void ToggleEnemyPanel()
    {
        if (EnemyShow)
        {
            EnemyPanel.SetActive(false);
            EnemyShow = false;
        }
        else
        {
            EnemyPanel.SetActive(true);
            EnemyShow = true;
        }
    }

    /// <summary>
    /// Inchide panoul de informatii suplimentare
    /// </summary>
    private void CloseAdditionalInfoPanel()
    {

        AdditionalInfoPanel.SetActive(false);
    }
}

