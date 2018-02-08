﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net;

namespace DarkRift.Server.Unity
{
    [CustomEditor(typeof(UnityServer))]
    [CanEditMultipleObjects]
    public class UnityClientEditor : Editor
    {
        UnityServer server;

        SerializedProperty createOnEnable;

        string address;
        SerializedProperty port;
        SerializedProperty ipVersion;
        SerializedProperty maxStrikes;

        SerializedProperty dataDirectory;

        SerializedProperty logToFile;
        SerializedProperty logFileString;
        SerializedProperty logToUnityConsole;
        SerializedProperty logToDebug;

        SerializedProperty loadByDefault;

        bool showServer, showData, showLogging, showPlugins, showDatabases;

        void OnEnable()
        {
            server = (UnityServer)serializedObject.targetObject;

            createOnEnable = serializedObject.FindProperty("createOnEnable");

            address         = server.Address.ToString();
            port            = serializedObject.FindProperty("port");
            ipVersion       = serializedObject.FindProperty("ipVersion");
            maxStrikes      = serializedObject.FindProperty("maxStrikes");

            dataDirectory   = serializedObject.FindProperty("dataDirectory");

            logToFile       = serializedObject.FindProperty("logToFile");
            logFileString   = serializedObject.FindProperty("logFileString");
            logToUnityConsole = serializedObject.FindProperty("logToUnityConsole");
            logToDebug      = serializedObject.FindProperty("logToDebug");

            loadByDefault   = serializedObject.FindProperty("loadByDefault");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(createOnEnable);

            if (showServer = EditorGUILayout.Foldout(showServer, "Server Setttings"))
            {
                EditorGUI.indentLevel++;

                //Display IP address
                address = EditorGUILayout.TextField(new GUIContent("Address", "The address the client will connect to."), address);

                try
                {
                    server.Address = IPAddress.Parse(address);
                }
                catch (FormatException)
                {
                    EditorGUILayout.HelpBox("Invalid IP address.", MessageType.Error);
                }
                
                EditorGUILayout.PropertyField(port);

                //Draw IP versions manually else it gets formatted as "Ip Version" and "I Pv4" -_-
                ipVersion.enumValueIndex = EditorGUILayout.Popup(new GUIContent("IP Version", "The IP protocol version the server will listen on."), ipVersion.enumValueIndex, Array.ConvertAll(ipVersion.enumNames, i => new GUIContent(i)));

                EditorGUILayout.PropertyField(maxStrikes);

                EditorGUI.indentLevel--;
            }

            if (showData = EditorGUILayout.Foldout(showData, "Data Setttings"))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(dataDirectory);

                EditorGUI.indentLevel--;
            }

            if (showLogging = EditorGUILayout.Foldout(showLogging, "Logging Setttings"))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(logToFile);

                EditorGUI.indentLevel++;
                if (logToFile.boolValue)
                    EditorGUILayout.PropertyField(logFileString);
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(logToUnityConsole);

                EditorGUILayout.PropertyField(logToDebug);

                EditorGUI.indentLevel--;
            }

            //Draw plugins list
            if (showPlugins = EditorGUILayout.Foldout(showPlugins, "Plugin Setttings"))
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(loadByDefault);

                IEnumerable<Type> types = UnityServerHelper.SearchForPlugins();

                foreach (Type type in types)
                {
                    ServerSpawnData.PluginsSettings.PluginSettings plugin = server.Plugins.SingleOrDefault(p => p.Type == type.Name);

                    if (plugin == null)
                    {
                        plugin = new ServerSpawnData.PluginsSettings.PluginSettings
                        {
                            Type = type.Name,
                            Load = true
                        };

                        server.Plugins.Add(plugin);
                    }
                    
                    EditorGUILayout.HelpBox("The following are plugins in your project, tick those to be loaded.", MessageType.Info);
                    
                    plugin.Load = EditorGUILayout.Toggle(type.Name, plugin.Load);
                    
                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;
            }

            //Draw databases manually
            if (showDatabases = EditorGUILayout.Foldout(showDatabases, "Databases"))
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < server.Databases.Count; i++)
                {
                    ServerSpawnData.DatabaseSettings.DatabaseConnectionData database = server.Databases[i];

                    database.Name = EditorGUILayout.TextField("Name", database.Name);

                    database.ConnectionString = EditorGUILayout.TextField("Connection String", database.ConnectionString);

                    Rect removeRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());        //So indent level affects the button
                    if (GUI.Button(removeRect, "Remove"))
                    {
                        server.Databases.Remove(database);
                        i--;
                    }

                    EditorGUILayout.Space();
                }

                Rect addRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(true));
                if (GUI.Button(addRect, "Add Database"))
                    server.Databases.Add(new ServerSpawnData.DatabaseSettings.DatabaseConnectionData("NewDatabase", "Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;"));

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
