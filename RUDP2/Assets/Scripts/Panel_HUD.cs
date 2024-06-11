
using UnityEngine;
using UnityEngine.UI;


public class Panel_HUD : MonoBehaviour
{
   public Button btnClient;
   public Button btnServer;
   public GameObject client;
   public GameObject server;

   private void Start()
   {
      btnClient.onClick.AddListener(BtnClientOnClick);
      btnServer.onClick.AddListener(BtnServerOnClick);
   }

   private void BtnServerOnClick()
   {
      btnServer.gameObject.SetActive(true);
      btnClient.gameObject.SetActive(false);
      server.SetActive(true);
   }

   private void BtnClientOnClick()
   {
      btnServer.gameObject.SetActive(false);
      btnClient.gameObject.SetActive(true);
      client.SetActive(true);
   }
}
