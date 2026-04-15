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
    [SerializeField] GameObject EnemyPanel;


    [Header("ToolTip")]
    [SerializeField] Tooltip ToolTip;

    private bool BoneShow;
    private bool MuscleShow;
    private bool Joints_OrgansShow;
    private bool EnemyShow;

    private void Awake()
    {
        Instance = this;
        MusclePanel.SetActive(false);
        BonePanel.SetActive(false);
        Joints_OrgansPanel.SetActive(false);
        EnemyPanel.SetActive(false);
        BoneShow = false;
        MuscleShow = false;
        Joints_OrgansShow = false;
        EnemyShow = false;
    }
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
        if (BoneShow == false && MuscleShow == false && Joints_OrgansShow == false)
        {
            ToolTip.Toggle(false);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleEnemyPanel();
        }
    }
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
            ToolTip.SetBoneSize();
        }
        else
        {
            BonePanel.SetActive(false);
            BoneShow = false;
        }
    }
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
            ToolTip.SetMuscleSize();
        }
        else
        {
            MusclePanel.SetActive(false);
            MuscleShow = false;
        }
    }
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
            ToolTip.SetJointOrganSize();
        }
        else
        {
            Joints_OrgansPanel.SetActive(false);
            Joints_OrgansShow = false;
        }
    }
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
}

