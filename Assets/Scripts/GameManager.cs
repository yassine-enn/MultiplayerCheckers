using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.ComponentModel.Design;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { set; get; }
  public GameObject mainMenu;
  public GameObject serverMenu;
  public GameObject connectMenu;
  public GameObject serverPrefab;
  public GameObject clientPrefab;
  public TMPro.TMP_InputField nameInput;

  private void Start()
  {
    Instance = this;
    serverMenu.SetActive(false);
    connectMenu.SetActive(false);
    DontDestroyOnLoad(gameObject);

  }
  public void ConnectButton()
  {

    mainMenu.SetActive(false);
    connectMenu.SetActive(true);
  }
  public void HostButton()
  {
    try
    {
      Server s = Instantiate(serverPrefab).GetComponent<Server>();
      s.Init();
      Client c = Instantiate(clientPrefab).GetComponent<Client>();
      c.clientName = nameInput.text;
      c.isHost = true;
      if (c.clientName == "") ;
      c.clientName = "Host";
      c.ConnectToServer("127.0.0.1", 6321);
    }
    catch (Exception e)
    {
      Debug.Log(e.Message);
    }
    mainMenu.SetActive(false);
    serverMenu.SetActive(true);
  }
  public void ConnectToServerButton()
  {
    string hostAddress = GameObject.Find("HostInput").GetComponent<TMPro.TMP_InputField>().text;
    if (hostAddress == "")
    {
      hostAddress = "127.0.0.1";
    }
    try
    {
      Debug.Log("hey");
      Client c = Instantiate(clientPrefab).GetComponent<Client>();
      c.clientName = nameInput.text;
      if (c.clientName == "") ;
      c.clientName = "Client";
      c.ConnectToServer(hostAddress, 6321);
      connectMenu.SetActive(false);
    }
    catch (Exception e)
    {
      Debug.Log(e.Message);
    }
  }

  public void BackButton()
  {
    mainMenu.SetActive(true);
    serverMenu.SetActive(false);
    connectMenu.SetActive(false);
    Server s = FindObjectOfType<Server>();
    if (s != null)
      Destroy(s.gameObject);
    Client c = FindObjectOfType<Client>();
    if (c != null)
      Destroy(c.gameObject);
  }
  public void HotseatButton()
  {
    SceneManager.LoadScene("Game");
  }
  public void StartGame()
  {
    SceneManager.LoadScene("Game");
  }

}
