using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts;

public class RegisterManager : MonoBehaviour {

    // Use this for initialization
    public InputField username;
    public InputField password;
    public InputField email;
    public Button submitButton;

    RegisterForm form;
	void Start () {
        username.onEndEdit.AddListener(SubmitName);
        password.onEndEdit.AddListener(SubmitPass);
        email.onEndEdit.AddListener(SubmitEmail);
        submitButton.onClick.AddListener(SubmitForm);
        form = new RegisterForm();

    }

    #region Acciones de input y boton
    private void SubmitForm()
    {
        string jsonForm = JsonUtility.ToJson(form);
        register(jsonForm);
        Debug.Log(jsonForm);
    }

    private void SubmitName(string user)
    {
        Debug.Log(user);
        form.username = user;
        

    }

    private void SubmitPass(string pass)
    {
        Debug.Log(pass);
        Debug.Log(password.text);
        form.password = pass;
    }

    private void SubmitEmail(string mail)
    {
        Debug.Log(mail);
        form.email = mail;
    }
    #endregion

    #region Acciones API
    void register(string newUser)
    {
        string requestUrl = "http://192.168.98.131:5000/register";
        StartCoroutine(Post(requestUrl,newUser));
    }

    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);
    }
#endregion 
}

