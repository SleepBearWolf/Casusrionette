using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked = true; // ʶҹТͧ��е�
    public string nextSceneName; // ���ͧ͢�չ�Ѵ�

    void OnMouseDown() // ����ͼ����蹤�ԡ����е�
    {
        if (!isLocked)
        {
            LoadNextScene(); // ��һ�е������ͤ �����Ŵ�չ�Ѵ�
        }
        else
        {
            Debug.Log("The door is locked!"); // ����һ�е���ͤ����
        }
    }

    void LoadNextScene()
    {
        // �� SceneManager ������Ŵ�չ�Ѵ�
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    public void UnlockDoor() // �ѧ��ѹ����Ѻ�Ŵ��ͤ��е�
    {
        isLocked = false;
        Debug.Log("The door has been unlocked!");
    }
}
