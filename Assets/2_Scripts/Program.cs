using System;
using UnityEngine;

public class Program : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Hello, World!");
        Publisher publisher = new Publisher();
        publisher.msg += ResultProcess;

        publisher.SendMessage("추가 문제 주세요!");

        Debug.Log("작업 완료!");
    }

    private void ResultProcess(string msg, int v)
    {
        throw new NotImplementedException();
    }

    void ResultProcess(string msg)
    {
        Debug.Log($"메시지를 수신했습니다. {msg}");
    }
}

public class Publisher
{
    public delegate void OnMessage(string msg ,int v);
    public event OnMessage msg;
    int v = 10;
    public void SendMessage(string text)
    {
        Debug.Log($"메시지를 방송합니다. {text}");

        msg?.Invoke(text, v);
    }
}