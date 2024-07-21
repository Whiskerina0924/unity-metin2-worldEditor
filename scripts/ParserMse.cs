using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using JosephEngine;
/// <summary>
/// class for importing jsons
/// </summary>
public class ParserMse : MonoBehaviour
{
    #region UI
    public Dropdown dd_mseItem;
    public Button btn_replay, btn_quit, btn_next, btn_before, btn_pause, btn_autoplay, btn_stop;
    public Toggle tg_showChar, tg_bSphere, tg_cursor;
    public RawImage ri_black;
    public Slider sli_simSpeed;
    public Text tt_simSpeed, tt_current;
    public InputField if_current;
    #endregion

    #region 3D
    public ParticleSystem particlePrefab;
    public GameObject go_char, go_bSphere, go_loading_icon;
    public GameObject[] go_cursors = new GameObject[3];
    public Animator anim_warning;
    #endregion

    int totalIndex = 0, pauseCnt = 0;
    /// <summary>
    /// initializes overall
    /// </summary>
    void Start()
    {
        Init_UI();
    }
    /// <summary>
    /// initializes UI
    /// </summary>
    void Init_UI()
    {
        dd_mseItem.AddOptions(GetAllMses($"config/jsons"));
        dd_mseItem.onValueChanged.AddListener(OnDropDown);
        btn_replay.onClick.AddListener(RePlay);
        btn_quit.onClick.AddListener(Quit);
        btn_next.onClick.AddListener(Next);
        btn_before.onClick.AddListener(Before);
        btn_pause.onClick.AddListener(Pause);
        btn_autoplay.onClick.AddListener(AutoPlay);
        btn_stop.onClick.AddListener(Stop);
        tg_showChar.onValueChanged.AddListener(ShowChar);
        tg_bSphere.onValueChanged.AddListener(ShowBsphere);
        tg_cursor.onValueChanged.AddListener(ShowCursor);
        sli_simSpeed.onValueChanged.AddListener(ChangeSimSpeed);
        if_current.onEndEdit.AddListener(OnEndEdit);
    }
    #region for UI
    /// <summary>
    /// get all json's url in the specified directory
    /// </summary>
    /// <param name="directory_url">specified directory that includes jsons</param>
    /// <returns>collection of urls</returns>
    List<string> GetAllMses(string directory_url)
    {
        List<string> all_mses = new List<string>();
        if (Directory.Exists(directory_url))
        {
            string[] urls = Directory.GetFiles(directory_url);
            foreach (string url in urls)
            {
                all_mses.Add(url);
            }
            tt_current.text = "/" + urls.Length;
        }
        return all_mses;
    }
    void OnEndEdit(string str)
    {
        if (string.IsNullOrEmpty(str)) return;
        int index = int.Parse(str);
        dd_mseItem.value = index;
    }
    void ChangeSimSpeed(float val)
    {
        Time.timeScale = val / 2f;
        tt_simSpeed.text = Time.timeScale.ToString("f1") + " x";
    }
    /// <summary>
    /// calls when "Pause" button is clicked
    /// </summary>
    void Pause()
    {
        pauseCnt++;
        if (pauseCnt % 2 == 1)
        {
            Color col;
            if (ColorUtility.TryParseHtmlString("#FF7F00", out col))
                btn_pause.GetComponent<Image>().color = col;
            btn_pause.GetComponentInChildren<Text>().text = "Play";
            btn_replay.enabled = false;
            Time.timeScale = 0f;
        }
        else
        {
            Color col;
            if (ColorUtility.TryParseHtmlString("#008cff", out col))
                btn_pause.GetComponent<Image>().color = col;
            btn_pause.GetComponentInChildren<Text>().text = "Pause";
            btn_replay.enabled = true;
            Time.timeScale = sli_simSpeed.value / 2f;
        }
    }
    /// <summary>
    /// calls when "AutoPlay" button is clicked
    /// </summary>
    void AutoPlay()
    {
        go_loading_icon.SetActive(true);
        StartCoroutine(PsAutoPlay());
    }
    /// <summary>
    /// AutoPlay coroutine for waiting and next
    /// </summary>
    /// <returns></returns>
    IEnumerator PsAutoPlay()
    {
        yield return new WaitForSeconds(1f);
        Next();
        StartCoroutine(PsAutoPlay());
    }
    /// <summary>
    /// calls when "Stop" button is called
    /// </summary>
    void Stop()
    {
        go_loading_icon.SetActive(false);
        StopAllCoroutines();
    }
    /// <summary>
    /// calls when Dropdown is opened
    /// </summary>
    /// <param name="index">dropdown's value</param>
    void OnDropDown(int index)
    {
        BroadcastMessage("SetPreviousIndex", index);
        if (index != 0)
        {
            if_current.text = dd_mseItem.value.ToString();
            RePlay();
        }
        MouseLook.isLocked = false;
    }
    /// <summary>
    /// show/hide character
    /// </summary>
    /// <param name="b">tg_showChar's value</param>
    void ShowChar(bool b)
    {
        if (b)
        {
            go_char.SetActive(true);
        }
        else
        {
            go_char.SetActive(false);
        }
    }
    /// <summary>
    /// show/hide sphere
    /// </summary>
    /// <param name="b">tg_bSphere's value</param>
    void ShowBsphere(bool b)
    {
        if (b)
        {
            go_bSphere.SetActive(true);
        }
        else
        {
            go_bSphere.SetActive(false);
        }
    }
    /// <summary>
    /// show/hide cursor
    /// </summary>
    /// <param name="b">tg_cursor's value</param>
    void ShowCursor(bool b)
    {
        if (b)
        {
            foreach (var item in go_cursors)
            {
                item.SetActive(true);
            }
        }
        else
        {
            foreach (var item in go_cursors)
            {
                item.SetActive(false);
            }
        }
    }
    /// <summary>
    /// moves to next effect
    /// </summary>
    void Next()
    {
        int cnt = dd_mseItem.options.Count - 1;
        if (dd_mseItem.value == cnt)
        {
            dd_mseItem.value = 0;
        }
        if_current.text = (++dd_mseItem.value).ToString();
    }
    /// <summary>
    /// moves to before effect
    /// </summary>
    void Before()
    {
        int cnt = dd_mseItem.options.Count - 1;
        if (dd_mseItem.value <= 1)
        {
            dd_mseItem.value = cnt;
            if_current.text = cnt.ToString();
        }
        else
            if_current.text = (--dd_mseItem.value).ToString();
    }
    /// <summary>
    /// repeats current effect
    /// </summary>
    void RePlay()
    {
        if (MouseLook.isLocked)
            MouseLook.isLocked = false;
        if (dd_mseItem.value != 0)
        {
            string str = File.ReadAllText(dd_mseItem.captionText.text);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(str);
            go_bSphere.transform.localScale = Vector3.one * playerData.boundingSphereRadius / 100f;
            go_bSphere.transform.position = playerData.boundingSpherePosition;

            foreach (Particle particle in playerData.particles)
                ParseParticleProperties(particle);
        }
        else
        {
            ri_black.enabled = true;
            StartCoroutine(Wait());
            anim_warning.enabled = true;
            anim_warning.Play("warning");
        }
    }
    /// <summary>
    /// coroutine for waitng error animation
    /// </summary>
    /// <returns></returns>
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        ri_black.enabled = false;
    }
    /// <summary>
    /// quit the Application in both of editor and standalone
    /// </summary>
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    #endregion
    /// <summary>
    /// create particle using parsed particle and emitter parameters
    /// </summary>
    /// <param name="particle"></param>
    #region for 3D

    void ParseParticleProperties(Particle particle)
    {
        switch (particle.billboardType)
        {
            case 4: // for BILLBOARD_TYPE_2FACE
                CreateParticle(
particle.startTime, particle.timeEventPosition, particle.maxEmissionCount, particle.cycleLength, particle.cycleLoopEnable, particle.loopCount, particle.emitterShape, particle.emitterEmitFromEdgeFlag,
particle.timeEventEmittingSize, particle.timeEventEmittingAngularVelocity, particle.timeEventEmittingDirectionX, particle.timeEventEmittingDirectionY, particle.timeEventEmittingDirectionZ,
particle.timeEventEmittingVelocity, particle.timeEventEmissionCountPerSecond, particle.timeEventLifeTime, particle.timeEventSizeX, particle.timeEventSizeY, particle.emittingRadius, particle.emittingSize,
particle.billboardType, particle.rotationType, particle.rotationSpeed, particle.timeEventScaleX, particle.timeEventScaleY, particle.timeEventColorRed, particle.timeEventColorGreen, particle.timeEventColorBlue,
particle.timeEventAlpha, particle.timeEventRotation, particle.textureFiles, -30f, 90f);
                CreateParticle(
particle.startTime, particle.timeEventPosition, particle.maxEmissionCount, particle.cycleLength, particle.cycleLoopEnable, particle.loopCount, particle.emitterShape, particle.emitterEmitFromEdgeFlag,
particle.timeEventEmittingSize, particle.timeEventEmittingAngularVelocity, particle.timeEventEmittingDirectionX, particle.timeEventEmittingDirectionY, particle.timeEventEmittingDirectionZ,
particle.timeEventEmittingVelocity, particle.timeEventEmissionCountPerSecond, particle.timeEventLifeTime, particle.timeEventSizeX, particle.timeEventSizeY, particle.emittingRadius, particle.emittingSize,
particle.billboardType, particle.rotationType, particle.rotationSpeed, particle.timeEventScaleX, particle.timeEventScaleY, particle.timeEventColorRed, particle.timeEventColorGreen, particle.timeEventColorBlue,
particle.timeEventAlpha, particle.timeEventRotation, particle.textureFiles, 30f, 90f);
                break;
            case 5: // for BILLBOARD_TYPE_3FACE
                for (int j = 0; j < 3; j++)
                {
                    CreateParticle(
    particle.startTime, particle.timeEventPosition, particle.maxEmissionCount, particle.cycleLength, particle.cycleLoopEnable, particle.loopCount, particle.emitterShape, particle.emitterEmitFromEdgeFlag,
    particle.timeEventEmittingSize, particle.timeEventEmittingAngularVelocity, particle.timeEventEmittingDirectionX, particle.timeEventEmittingDirectionY, particle.timeEventEmittingDirectionZ,
    particle.timeEventEmittingVelocity, particle.timeEventEmissionCountPerSecond, particle.timeEventLifeTime, particle.timeEventSizeX, particle.timeEventSizeY, particle.emittingRadius, particle.emittingSize,
    particle.billboardType, particle.rotationType, particle.rotationSpeed, particle.timeEventScaleX, particle.timeEventScaleY, particle.timeEventColorRed, particle.timeEventColorGreen, particle.timeEventColorBlue,
    particle.timeEventAlpha, particle.timeEventRotation, particle.textureFiles, 60f * j, 90f);
                }
                break;
            default: // for the other
                CreateParticle(
particle.startTime, particle.timeEventPosition, particle.maxEmissionCount, particle.cycleLength, particle.cycleLoopEnable, particle.loopCount, particle.emitterShape, particle.emitterEmitFromEdgeFlag,
particle.timeEventEmittingSize, particle.timeEventEmittingAngularVelocity, particle.timeEventEmittingDirectionX, particle.timeEventEmittingDirectionY, particle.timeEventEmittingDirectionZ,
particle.timeEventEmittingVelocity, particle.timeEventEmissionCountPerSecond, particle.timeEventLifeTime, particle.timeEventSizeX, particle.timeEventSizeY, particle.emittingRadius, particle.emittingSize,
particle.billboardType, particle.rotationType, particle.rotationSpeed, particle.timeEventScaleX, particle.timeEventScaleY, particle.timeEventColorRed, particle.timeEventColorGreen, particle.timeEventColorBlue,
particle.timeEventAlpha, particle.timeEventRotation, particle.textureFiles, 0f, 0f);
                break;
        }

        #endregion
    }
    void CreateParticle(
        float startTime, SerializableDictionary<float, Vector3> timeEventPosition, int maxEmissionCount, float cycleLength, int cycleLoopEnable, int loopCount, int emitterShape, int emitterEmitFromEdgeFlag,
        SerializableDictionary<float, float> timeEventEmittingSize, SerializableDictionary<float, float> timeEventEmittingAngularVelocity, SerializableDictionary<float, float> timeEventEmittingDirectionX, SerializableDictionary<float, float> timeEventEmittingDirectionY, SerializableDictionary<float, float> timeEventEmittingDirectionZ,
        SerializableDictionary<float, float> timeEventEmittingVelocity, SerializableDictionary<float, float> timeEventEmissionCountPerSecond, SerializableDictionary<float, float> timeEventLifeTime, SerializableDictionary<float, float> timeEventSizeX, SerializableDictionary<float, float> timeEventSizeY, float emittingRadius, Vector3 emittingSize,
        int billboardType, int rotationType, float rotationSpeed, SerializableDictionary<float, float> timeEventScaleX, SerializableDictionary<float, float> timeEventScaleY, SerializableDictionary<float, float> timeEventColorRed, SerializableDictionary<float, float> timeEventColorGreen, SerializableDictionary<float, float> timeEventColorBlue,
        SerializableDictionary<float, float> timeEventAlpha, SerializableDictionary<float, float> timeEventRotation, string textureFiles, float startRotation, float angle)
    {
        ParticleSystem ps = Instantiate(particlePrefab, Vector3.zero, particlePrefab.transform.rotation);
        ps.transform.Rotate(angle, 0f, 0f);
        ps.transform.localRotation *= Quaternion.Euler(0f, startRotation, 0f);

        #region main module // for controlling main properties of particle system such as duration, startDelay, startsize, startlifetime...
        var main = ps.main;
        if (cycleLoopEnable == 1)
        {
            if (loopCount != 0)
                main.duration = cycleLength * loopCount;
        }
        else
            main.duration = cycleLength;
        main.startDelay = startTime;
        main.maxParticles = maxEmissionCount;
        AnimationCurve curve = new AnimationCurve();
        foreach (var item in timeEventLifeTime.ToDictionary())
            curve.AddKey(item.Key, item.Value);
        //main.startLifetime = new ParticleSystem.MinMaxCurve(1f, curve);
        main.startLifetime = new ParticleSystem.MinMaxCurve(timeEventLifeTime.ToDictionary().Values.ToArray()[0]);
        main.startSize3D = true;
        curve = new AnimationCurve();
        foreach (var item in timeEventSizeX.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        main.startSizeX = new ParticleSystem.MinMaxCurve(1f, curve);
        curve = new AnimationCurve();
        foreach (var item in timeEventSizeY.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        main.startSizeY = new ParticleSystem.MinMaxCurve(1f, curve);
        #endregion

        #region emission module // for controlling each particle's emission rate...
        var emission = ps.emission;
        emission.enabled = true;
        curve = new AnimationCurve();
        foreach (var item in timeEventEmissionCountPerSecond.ToDictionary())
            curve.AddKey(item.Key, item.Value);
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(1f, curve);
        #endregion

        #region shape module // for controlling of each particle's emitter shape, radius, radiusThichness, scale...
        var shape = ps.shape;
        shape.enabled = true;
        switch (emitterShape)
        {
            case 0: // EMITTER_SHAPE_POINT
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 0.0001f;
                shape.radiusThickness = 0f;
                break;
            case 1: // EMITTER_SHAPE_ELLIPSE
                shape.shapeType = ParticleSystemShapeType.Circle;
                if (emitterEmitFromEdgeFlag == 1)
                {
                    shape.radiusThickness = 0f;
                    try
                    {
                        shape.radius = (emittingRadius + timeEventEmittingSize.ToDictionary().Values.ToArray()[0]);
                        if (timeEventEmittingSize.ToDictionary().Count > 1)
                        {
                            int _index = 0;
                            foreach (var item in timeEventEmittingSize.ToDictionary())
                            {
                                if (_index != 0)
                                {
                                    StartCoroutine(SetEmittingSize(ps.shape, item.Key, item.Value, emittingRadius));
                                }
                                _index++;
                            }
                        }
                    }
                    catch
                    {
                        shape.radius = emittingRadius;
                    }
                }
                else
                {
                    shape.radiusThickness = 1f;
                    try
                    {
                        shape.radius = (UnityEngine.Random.Range(0f, emittingRadius) + timeEventEmittingSize.ToDictionary().Values.ToArray()[0]);
                        if (timeEventEmittingSize.ToDictionary().Count > 1)
                        {
                            int _index = 0;
                            foreach (var item in timeEventEmittingSize.ToDictionary())
                            {
                                if (_index != 0)
                                {
                                    StartCoroutine(SetEmittingSize(ps.shape, item.Key, item.Value, emittingRadius));
                                }
                                _index++;
                            }
                        }
                    }
                    catch
                    {
                        shape.radius = UnityEngine.Random.Range(0f, emittingRadius);
                    }
                }
                break;
            case 2: // EMITTER_SHAPE_SQUARE
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = emittingSize;
                break;
            case 3: // EMITTER_SHAPE_SPHERE
                shape.shapeType = ParticleSystemShapeType.Sphere;
                if (emitterEmitFromEdgeFlag == 1)
                {
                    shape.radiusThickness = 0f;
                    try
                    {
                        shape.radius = (emittingRadius + timeEventEmittingSize.ToDictionary().Values.ToArray()[0]);
                        if (timeEventEmittingSize.ToDictionary().Count > 1)
                        {
                            int _index = 0;
                            foreach (var item in timeEventEmittingSize.ToDictionary())
                            {
                                if (_index != 0)
                                {
                                    StartCoroutine(SetEmittingSize(ps.shape, item.Key, item.Value, emittingRadius));
                                }
                                _index++;
                            }
                        }
                    }
                    catch
                    {
                        shape.radius = emittingRadius;
                    }
                }
                else
                {
                    shape.radiusThickness = 1f;
                    try
                    {
                        shape.radius = (UnityEngine.Random.Range(0f, emittingRadius) + timeEventEmittingSize.ToDictionary().Values.ToArray()[0]);
                        if (timeEventEmittingSize.ToDictionary().Count > 1)
                        {
                            int _index = 0;
                            foreach (var item in timeEventEmittingSize.ToDictionary())
                            {
                                if (_index != 0)
                                {
                                    StartCoroutine(SetEmittingSize(ps.shape, item.Key, item.Value, emittingRadius));
                                }
                                _index++;
                            }
                        }
                    }
                    catch
                    {
                        shape.radius = UnityEngine.Random.Range(0f, emittingRadius);
                    }
                }
                break;
            default: break;
        }
        #endregion

        #region velocityOverLifetime module // for controlling of each particle's velocity(X, Y, Z) over time
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        curve = new AnimationCurve();
        foreach (var item in timeEventEmittingDirectionX.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, curve);
        curve = new AnimationCurve();
        foreach (var item in timeEventEmittingDirectionY.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-1f, curve);
        curve = new AnimationCurve();
        foreach (var item in timeEventEmittingDirectionZ.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(1f, curve);
        curve = new AnimationCurve();
        foreach (var item in timeEventEmittingVelocity.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        try
        {
            velocityOverLifetime.speedModifierMultiplier = timeEventEmittingVelocity.ToDictionary().Values.ToArray()[0];
        }
        catch
        {
            velocityOverLifetime.speedModifierMultiplier = 1f;
        }

        curve = new AnimationCurve();
        foreach (var item in timeEventEmittingAngularVelocity.ToDictionary())
        {
            curve.AddKey(item.Key, item.Value);
        }
        try
        {
            velocityOverLifetime.orbitalZ = timeEventEmittingAngularVelocity.ToDictionary().Values.ToArray()[0];
        }
        catch
        {
            velocityOverLifetime.orbitalZ = 0f;
        }
        #endregion

        #region colorOverLifetime module // for controlling color and alpha of each particle over time
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Blend;
        GradientColorKey[] colorKeys = new GradientColorKey[timeEventColorRed.ToDictionary().Count];
        int index = 0;
        foreach (var pair in timeEventColorRed.ToDictionary())
        {
            try
            {
                colorKeys[index] = new GradientColorKey(new Color(pair.Value, timeEventColorGreen.ToDictionary().Values.ToArray()[index], timeEventColorBlue.ToDictionary().Values.ToArray()[index]), pair.Key);
                index++;
            }
            catch { }
        }

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[timeEventAlpha.ToDictionary().Count];
        index = 0;
        foreach (var pair in timeEventAlpha.ToDictionary())
        {
            alphaKeys[index] = new GradientAlphaKey(pair.Value, pair.Key);
            index++;
        }

        gradient.SetKeys(colorKeys, alphaKeys);
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        #endregion

        #region sizeOverLifetime module // for controlling of each particle's size(X and Y) over time
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.separateAxes = true;

        curve = new AnimationCurve();
        foreach (var pair in timeEventScaleX.ToDictionary())
        {
            curve.AddKey(pair.Key, pair.Value);
        }
        sizeOverLifetime.x = new ParticleSystem.MinMaxCurve(1f, curve);

        curve = new AnimationCurve();
        foreach (var pair in timeEventScaleY.ToDictionary())
        {
            curve.AddKey(pair.Key, pair.Value);
        }
        sizeOverLifetime.y = new ParticleSystem.MinMaxCurve(1f, curve);

        #endregion

        #region rotationOverLifetime module // for controlling of entire partilce system's rotation state over time
        var rotationOverLifetime = ps.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        curve = new AnimationCurve();
        foreach (var pair in timeEventRotation.ToDictionary())
        {
            curve.AddKey(pair.Key, pair.Value);
        }
        if (billboardType == 2)
            rotationOverLifetime.y = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad, curve);
        else
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad, curve);
        #endregion

        #region rotationBySpeed module // for controlling of each particle's rotation speed around Z axis
        var rotationBySpeed = ps.rotationBySpeed;
        rotationBySpeed.enabled = true;
        var _rotationSpeed = rotationSpeed * Mathf.Deg2Rad;
        switch (rotationType)
        {
            case 0:
                rotationBySpeed.zMultiplier = 0f; break;
            case 2:
                rotationBySpeed.zMultiplier = 1f * _rotationSpeed; break;
            case 3:
                rotationBySpeed.zMultiplier = -1f * _rotationSpeed; break;
            default:
                break;
        }
        #endregion

        #region renderer module // for controlling renderer parameters such as billboard, material(textureFiles), alignment...
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        switch (billboardType)
        {
            case 0: // BILLBOARD_TYPE_NONE
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.alignment = ParticleSystemRenderSpace.Local; break;
            case 1: // BILLBOARD_TYPE_ALL
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.alignment = ParticleSystemRenderSpace.View; break;
            case 2: // BILLBOARD_TYPE_Y
                renderer.renderMode = ParticleSystemRenderMode.VerticalBillboard;
                renderer.alignment = ParticleSystemRenderSpace.Local; break;
            case 3: // BILLBOARD_TYPE_LIE
                renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
                renderer.alignment = ParticleSystemRenderSpace.Local; break;
            case 4: // BILLBOARD_TYPE_2FACE
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.alignment = ParticleSystemRenderSpace.Local; break;
            case 5: // BILLBOARD_TYPE_3FACE
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.alignment = ParticleSystemRenderSpace.Local; break;
            default: break;
        }
        Texture tex_mat = Resources.Load<Texture>(textureFiles);
        renderer.material.mainTexture = tex_mat;
        #endregion

        #region custom animation  //for position animating of particle system
        Animation anim = ps.GetComponent<Animation>();
        AnimationCurve curve_pos_x = new AnimationCurve(), curve_pos_y = new AnimationCurve(), curve_pos_z = new AnimationCurve();

        AnimationClip clip = new AnimationClip();
        totalIndex++;
        clip.name = "anim" + totalIndex;
        clip.legacy = true;

        foreach (var pair in timeEventPosition.ToDictionary())
        {
            curve_pos_x.AddKey(pair.Key, pair.Value.x);
            curve_pos_y.AddKey(pair.Key, pair.Value.y);
            curve_pos_z.AddKey(pair.Key, pair.Value.z);
        }

        clip.SetCurve("", typeof(Transform), "localPosition.x", curve_pos_x);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curve_pos_y);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curve_pos_z);

        anim.AddClip(clip, clip.name);
        #endregion

        ps.Play();
        anim.Play(clip.name);
        Destroy(ps.gameObject, cycleLength + timeEventLifeTime.ToDictionary().Values.ToArray()[0] + 1f);
    }
    IEnumerator SetEmittingSize(ParticleSystem.ShapeModule shape, float key, float value, float emittingRadius)
    {
        yield return new WaitForSeconds(key);
        shape.radius = (emittingRadius + value);
    }
}