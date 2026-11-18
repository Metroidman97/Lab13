#if UNITY_EDITOR
using System.Xml.Linq;
using UnityEditor;
#endif
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    // xml urls
    public const string orlandoString = "http://api.openweathermap.org/data/2.5/weather?q=Orlando,us&mode=xml&appid=c3d9363a656059d0f57a915c349a70d9";
    public const string londonString = "https://api.openweathermap.org/data/2.5/weather?q=London,uk&mode=xml&appid=c3d9363a656059d0f57a915c349a70d9";
    public const string cairoString = "https://api.openweathermap.org/data/2.5/weather?q=Cairo,eg&mode=xml&appid=c3d9363a656059d0f57a915c349a70d9";
    public const string sydneyString = "https://api.openweathermap.org/data/2.5/weather?q=Sydney,au&mode=xml&appid=c3d9363a656059d0f57a915c349a70d9";
    public const string mumbaiString = "https://api.openweathermap.org/data/2.5/weather?q=Mumbai,in&mode=xml&appid=c3d9363a656059d0f57a915c349a70d9";

    // directional light
    public Light skyLight;

    // skybox materials
    public Material daySkybox;
    public Material nightSkybox;
    public Material rainySkybox;
    public Material snowySkybox;

    private WeatherManager weatherManager = new WeatherManager();

    [CityPicker(cities = new string[] { "Orlando", "London", "Cairo", "Sydney", "Mumbai" })]
    public string city;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadWeatherData(string url)
    {
        StartCoroutine(weatherManager.GetWeatherXML(OnXMLDataLoaded, url));
    }

    public void OnXMLDataLoaded(string data)
    {
        //Debug.Log(data);

        float temperature = 0;
        string precipitation = null;

        XDocument xml = XDocument.Parse(data);

        foreach (XElement item in xml.Elements())
        {
            temperature = (float)item.Element("temperature").Attribute("value");
            precipitation = (string)item.Element("precipitation").Attribute("mode");
            //Debug.Log(temperature + " " + precipitation);
        }

        Debug.Log(temperature + " " + precipitation);
    }

    public string GetCityUrl(string city)
    {
        switch (city)
        {
            case "Orlando":
                return orlandoString;
            case "London":
                return londonString;
            case "Cairo":
                return cairoString;
            case "Sydney":
                return sydneyString;
            case "Mumbai":
                return mumbaiString;
            default:
                Debug.Log("Invalid city name");
                return null;
        }
    }
}

public class CityPickerAttribute : PropertyAttribute
{
    public string[] cities;
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeatherController))]
public class WeatherControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var t = target as WeatherController;
        base.OnInspectorGUI();

        if (GUILayout.Button("Get city weather data"))
        {
            t.LoadWeatherData(t.GetCityUrl(t.city));
        }
    }
}

[CustomPropertyDrawer(typeof(CityPickerAttribute))]
public class CityPickerAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (CityPickerAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        var propertyRect = new Rect(position.x, position.y, position.width - 20, position.height);
        var dropdownButtonRect = new Rect(propertyRect.xMax, position.y, 20, position.height);

        EditorGUI.PropertyField(propertyRect, property);

        if (GUI.Button(dropdownButtonRect, "..."))
        {
            var menu = new GenericMenu();
            foreach (var option in attr.cities)
            {
                menu.AddItem(new GUIContent(option.ToString()), false, () =>
                {
                    property.stringValue = option.ToString();
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }
}
#endif
