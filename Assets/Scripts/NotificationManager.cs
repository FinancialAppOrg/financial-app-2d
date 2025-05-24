using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public bool dailyNotificationsEnabled = true;
    public bool inactivityNotificationsEnabled = true;

    private DateTime lastActivityTime;
    private TimeSpan inactivityThreshold = TimeSpan.FromSeconds(5); //TimeSpan.FromDays(1); 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
      
        dailyNotificationsEnabled = PlayerPrefs.GetInt("DailyNotifications", 1) == 1;
        inactivityNotificationsEnabled = PlayerPrefs.GetInt("InactivityNotifications", 1) == 1;


        ScheduleDailyNotification();
    }

    private void Update()
    {
        if (inactivityNotificationsEnabled && DateTime.Now - lastActivityTime > inactivityThreshold)
        {
            SendInactivityNotification();
            lastActivityTime = DateTime.Now; 
        }
    }

    public void RegisterActivity()
    {
        lastActivityTime = DateTime.Now; 
    }

    private void ScheduleDailyNotification()
    {
        if (dailyNotificationsEnabled)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = "daily_notifications",
                Name = "Notificaciones Diarias",
                Importance = Importance.Default,
                Description = "Notificaciones enviadas diariamente",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var notification = new AndroidNotification
            {
                Title = "Esta es tu señal",
                Text = "¡No olvides ingresar hoy! Continua aprendiendo con Gamifi",
                SmallIcon = "icono_llamita",
                LargeIcon = "llamita_icon_large",
                FireTime = DateTime.Now.AddSeconds(10) 
            };

            AndroidNotificationCenter.SendNotification(notification, "daily_notifications");
            Debug.Log("Notificación diaria programada.");
            //Debug.Log("Notificación diaria programada.");
            //StartCoroutine(SendDailyNotificationTest());
        
        }
    }

    private IEnumerator SendDailyNotificationTest()
    {
        while (dailyNotificationsEnabled)
        {
            Debug.Log("Notificación diaria enviada (prueba).");
            yield return new WaitForSeconds(10); 
        }
    }

    private void SendInactivityNotification()
    {
        //Debug.Log("Notificación por inactividad enviada.");
        if (inactivityNotificationsEnabled)
        {
            var notification = new AndroidNotification
            {
                Title = "¡Te extrañamos!",
                Text = "Parece que no has usado Gamifi en un tiempo. ¡Vuelve pronto!",
                SmallIcon = "icono_llamita",
                LargeIcon = "llamita_icon_large",
                FireTime = DateTime.Now 
            };

            AndroidNotificationCenter.SendNotification(notification, "daily_notifications");
            Debug.Log("Notificación por inactividad enviada.");
        }
    }

    public void EnableDailyNotifications(bool enabled)
    {
        dailyNotificationsEnabled = enabled;

        PlayerPrefs.SetInt("DailyNotifications", enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (enabled)
        {
            ScheduleDailyNotification();
        }
        else
        {
            Debug.Log("Notificaciones diarias desactivadas.");
        }
    }

    public void EnableInactivityNotifications(bool enabled)
    {
        inactivityNotificationsEnabled = enabled;
        PlayerPrefs.SetInt("InactivityNotifications", enabled ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log(enabled ? "Notificaciones por inactividad activadas." : "Notificaciones por inactividad desactivadas.");
    }
}