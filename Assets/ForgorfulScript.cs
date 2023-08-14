using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForgorfulScript : MonoBehaviour
{
    private int _id = ++_idc;
    private static int _idc;

    private string[] _ignored;
    private string[] _ignoredForForce;
    KMBombInfo _info;
    private bool _shouldShuffle = true, _isSolved, _useTimer = true;
    private List<ForgorfulScript> _others;
    private SubmissionState _submissionState = SubmissionState.Unstarted;
    //private int _prevSolved = -1;
    private int _prevSolvedAll = -1;
    private bool _force;
    private List<Transform> _allAnchors = new List<Transform>();
    private Dictionary<Transform, object> _bombFaces = new Dictionary<Transform, object>();
    private List<Component> _allShufflable = new List<Component>(), _timerRequired = new List<Component>();
    private Dictionary<Component, Transform> _currentParents = new Dictionary<Component, Transform>();
    private Dictionary<ForgorfulScript, Transform> _originalTransform = new Dictionary<ForgorfulScript, Transform>();
    private Dictionary<ForgorfulScript, Component> _originalComponent = new Dictionary<ForgorfulScript, Component>();
    private Dictionary<ForgorfulScript, int> _correctCounts = new Dictionary<ForgorfulScript, int>();
    private GameObject _template;
    private Type _selType, _selArrType;
    private System.Reflection.FieldInfo _selChildren, _selParent;
    private System.Reflection.MethodInfo _selDICSA, _selInit;
    private Component _timer;
    private object _timerFace;

    //private void Awake()
    //{
    //    Type type = ReflectionHelper.FindTypeInGame("ExcludeFromStaticBatch");
    //    UnityEngine.Object[] bombs = FindObjectsOfType(ReflectionHelper.FindTypeInGame("Bomb"));
    //    foreach (UnityEngine.Object bomb in bombs)
    //        for (int i = 0; i < ((MonoBehaviour)bomb).transform.childCount; ++i)
    //            for (int j = 0; j < ((MonoBehaviour)bomb).transform.GetChild(i).childCount; ++j)
    //                if (!((MonoBehaviour)bomb).transform.GetChild(i).GetChild(j).GetComponent(type))
    //                    ((MonoBehaviour)bomb).transform.GetChild(i).GetChild(j).gameObject.AddComponent(type);
    //}

    private IEnumerator Start()
    {
        Transform timerTr = null;
        if (_shouldShuffle)
        {
            _others = transform.root.GetComponentsInChildren<ForgorfulScript>().Where(f => _id != f._id).ToList();
            foreach (ForgorfulScript f in _others)
                f._shouldShuffle = false;

            if (_others.Count != 0)
            {
                List<string> stuff = new List<string>(_others.Count + 1);
                for (int i = 0; i < _others.Count + 1; ++i)
                {
                    string lab;
                    do
                    {
                        lab = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".OrderBy(_ => UnityEngine.Random.value).Take(3).Join("");
                    }
                    while (stuff.Contains(lab));
                    if (i < _others.Count)
                        _others[i].GetComponentInChildren<TextMesh>().text = lab;
                    else
                        GetComponentInChildren<TextMesh>().text = lab;
                }
            }

            // Ignore list: [Standard ignore list for when to become definitely solvable] [empty string] [ignore list for modules not to be shuffled when there are more than one of them]
            //"+SolvesAtEnd",
            //"+TimeDependent",
            //"",
            //"+SolvesAtEnd",
            //"+NeedsOtherSolves",
            //"+TimeDependent",
            //"Censorship",
            //"-The Swan",
            //"-Divided Squares",
            //"-Four-Card Monte",
            //"-Mystery Module",
            //"-Multitask",
            //"-42",
            //"-501",
            //"-Button Messer",
            //"-Custom Keys",
            //"-Simon",
            //"-Speedrun",
            //"-Peek-A-Boo",
            //"-Channel Surfing",
            //"-Damocles Lumber",
            //"-Module Maneuvers",
            //"-Purgatory",
            //"-Organization",
            //"-Übermodule",
            //"-The Twin",
            //"-Iconic",
            //"-Kugelblitz",
            //"-Don't Touch Anything",
            //"-Cube Synchronization",
            //"-Out of Time",
            //"-Tetrahedron",
            //"-Twister",
            //"-8",
            //"-Remember Simple",
            //"-Reporting Anomalies",
            //"-Watch the Clock"
            string[] ignored = GetComponent<KMBossModule>().GetIgnoredModules("I Forgor 💀", new string[]
                    {
                "I Forgor 💀", "Forget Me Not", "Souvenir", "Forget Everything", "Simon's Stages", "Forget This", "Purgatory", "The Troll", "Forget Them All", "Tallordered Keys", "Forget Enigma", "Forget Us Not", "Forget Perspective", "Organization", "The Very Annoying Button", "Forget Me Later", "Übermodule", "Ultimate Custom Night", "14", "Forget It Not", "Simon Forgets", "Brainf---", "Forget The Colors", "RPS Judging", "The Twin", "Iconic", "OmegaForget", "Kugelblitz", "A>N<D", "Don't Touch Anything", "Busy Beaver", "Whiteout", "Forget Any Color", "Keypad Directionality", "Security Council", "Shoddy Chess", "Floor Lights", "Black Arrows", "Forget Maze Not", "+", "Soulscream", "Cube Synchronization", "Out of Time", "Tetrahedron", "The Board Walk", "Gemory", "Duck Konundrum", "Concentration", "Twister", "Forget Our Voices", "Soulsong", "ID Exchange", "8", "Remember Simple", "Remembern't Simple", "The Grand Prix", "Forget Me Maybe", "HyperForget", "Bitwise Oblivion", "Damocles Lumber", "Top 10 Numbers", "Queen's War", "Forget Fractal", "Pointer Pointer", "Slight Gibberish Twist", "Piano Paradox", "OMISSION", "In Order", "The Nobody's Code", "Perspective Stacking", "Reporting Anomalies", "Forgetle", "Actions and Consequences", "FizzBoss", "Watch the Clock", "Solve Shift", "Turn The Key", "The Time Keeper", "Timing is Everything", "Bamboozling Time Keeper", "Password Destroyer", "OmegaDestroyer", "Zener Cards", "Doomsday Button", "Red Light Green Light",
                "",
                "+", "14", "A>N<D", "Actions and Consequences", "Amnesia", "B-Machine", "Bamboozling Time Keeper", "Binary Memory", "Bitwise Oblivion", "Black Arrows", "Brainf---", "Busy Beaver", "Concentration", "Censorship", "Cookie Jars", "Doomsday Button", "Duck Konundrum", "Encryption Bingo", "FizzBoss", "Floor Lights", "Forget Any Color", "Forget Enigma", "Forget Everything", "Forget Fractal", "Forget Infinity", "Forget It Not", "Forget Maze Not", "Forget Me Later", "Forget Me Maybe", "Forget Me Not", "Forget Our Voices", "Forget Perspective", "Forget The Colors", "Forget Them All", "Forget This", "Forget Us Not", "Forgetle", "Gemory", "Hogwarts", "HyperForget", "ID Exchange", "In Order", "Keypad Directionality", "OMISSION", "OmegaDestroyer", "OmegaForget", "Password Destroyer", "Perspective Stacking", "Piano Paradox", "Pointer Pointer", "Queen's War", "RPS Judging", "Red Light Green Light", "Remembern't Simple", "Scrabble Scramble", "Security Council", "Shoddy Chess", "Simon Forgets", "Simon's Stages", "Slight Gibberish Twist", "Solve Shift", "Soulscream", "Soulsong", "Souvenir", "Tallordered Keys", "The Board Walk", "The Generator", "The Grand Prix", "The Klaxon", "The Nobody's Code", "The Time Keeper", "The Troll", "The Very Annoying Button", "Timing is Everything", "Top 10 Numbers", "Turn The Key", "Turn The Keys", "Ultimate Custom Night", "Whiteout", "Zener Cards"
                    });
            _info = GetComponent<KMBombInfo>();
            int sep = Array.IndexOf(ignored, "");
            _ignoredForForce = ignored.Take(sep).ToArray();

            List<string> onBomb = _info.GetModuleNames();

            string[] dups = onBomb.Where(n => onBomb.Count(s => s == n) > 1).ToArray();

            if (onBomb.Contains("Squad's Shadow"))
            {
                string[] ssIgnore = GetComponent<KMBossModule>().GetIgnoredModules("Squad's Shadow");
                List<string> l3 = ignored.Skip(sep + 1).ToList();
                _ignored = dups.Where(n => !ssIgnore.Contains(n) || l3.Contains(n)).ToArray();
            }
            else
            {
                List<string> l3 = ignored.Skip(sep + 1).ToList();
                _ignored = dups.Where(n => l3.Contains(name)).ToArray();
            }

            Type t = ReflectionHelper.FindTypeInGame("BombComponent");
            Type tw = ReflectionHelper.FindTypeInGame("WidgetComponent"); //Vanilla widgets suck
            System.Reflection.MethodInfo m = t.Method("GetModuleDisplayName");
            System.Reflection.FieldInfo fi = t.GetField("ComponentType", ReflectionHelper.Flags);
            _template = transform.GetChild(0).gameObject;
            foreach (Component c in transform.root.GetComponentsInChildren(t))
            {
                if (!c.gameObject.activeInHierarchy)
                    continue;
                if (tw.IsAssignableFrom(c.GetType()))
                    continue;
                if ((int)fi.GetValue(c) == 0)
                    continue;
                string name = (string)m.Invoke(c, new object[0]);
                if (_ignored.Contains(name))
                {
                    if (_useTimer && t.Field<bool>("RequiresTimerVisibility", c))
                        _useTimer = false;
                    continue;
                }

                if (name == "Timer")
                {
                    _timer = c;
                    continue;
                }

                Debug.LogFormat("[I Forgor 💀 #{0}] An object called \"{1}\" will be shuffled.", _id, name);

                _allShufflable.Add(c);

                GameObject tr = Instantiate(_template, c.transform, false);
                tr.transform.SetParent(c.transform.parent, true);
                _currentParents[c] = tr.transform;
                _allAnchors.Add(tr.transform);

                ForgorfulScript s;
                if (s = c.GetComponent<ForgorfulScript>())
                {
                    _originalComponent.Add(s, c);
                    _originalTransform.Add(s, tr.transform);
                    _correctCounts[s] = 0;
                }

                if (t.Field<bool>("RequiresTimerVisibility", c))
                    _timerRequired.Add(c);
            }

            if (_useTimer)
            {
                Debug.LogFormat("[I Forgor 💀 #{0}] An object called \"Timer\" will be shuffled.", _id);
                _allShufflable.Add(_timer);

                GameObject tr = Instantiate(_template, _timer.transform, false);
                tr.transform.SetParent(_timer.transform.parent, true);
                timerTr = _currentParents[_timer] = tr.transform;
                _allAnchors.Add(tr.transform);
            }

            Component bimb = GetComponentInParent(ReflectionHelper.FindTypeInGame("Bomb"));
            _selType = ReflectionHelper.FindTypeInGame("Selectable");
            _selArrType = _selType.MakeArrayType();
            _selChildren = _selType.GetField("Children", ReflectionHelper.Flags);
            _selParent = _selType.GetField("Parent", ReflectionHelper.Flags);
            _selDICSA = _selType.Method("DeactivateImmediateChildSelectableAreas");
            _selInit = _selType.Method("Init");

            IList faces = bimb.GetType().Field<IList>("Faces", bimb);
            foreach (object face in faces)
            {
                List<Transform> anchors = face.GetType().Field<List<Transform>>("Anchors", face);
                foreach (Transform a in anchors)
                    foreach (Transform ca in _allAnchors)
                        if ((a.position - ca.position).magnitude < 0.05)
                            _bombFaces[ca] = ((MonoBehaviour)face).GetComponent(_selType);
                foreach (Transform a in anchors)
                    if ((a.position - _timer.transform.position).magnitude < 0.05)
                        _timerFace = face;
            }

            _allShufflable = _allShufflable.OrderByDescending(tr => _timerRequired.Contains(tr)).ToList();

            StartCoroutine(WatchSolves());
        }

        KMSelectable[] btns = GetComponent<KMSelectable>().Children;
        btns[0].OnInteract += () => { Press(0); return false; };
        btns[1].OnInteract += () => { Press(1); return false; };

        Debug.LogFormat("[I Forgor 💀 #{0}] I forgor 💀", _id, name);

        if (_shouldShuffle)
        {
            yield return null;
            yield return null;
            Type tt = ReflectionHelper.FindType("BombTimer"); // Bomb timer modifier sucks
            Component btm;
            if (tt != null && (btm = transform.root.GetComponentInChildren(tt)))
            {
                if (_useTimer)
                {
                    Debug.LogFormat("[I Forgor 💀 #{0}] The timer has been modified.", _id);
                    _allShufflable.Remove(_timer);
                    _allShufflable.Add(btm);

                    _currentParents[btm] = timerTr;
                }
                Component bimb = GetComponentInParent(ReflectionHelper.FindTypeInGame("Bomb"));
                IList faces = bimb.GetType().Field<IList>("Faces", bimb);
                foreach (object face in faces)
                {
                    List<Transform> anchors = face.GetType().Field<List<Transform>>("Anchors", face);
                    foreach (Transform a in anchors)
                        if ((a.position - btm.transform.position).magnitude < 0.05)
                            _timerFace = face;
                }
                _timer = btm;
            }
        }
    }

    private void Press(int v)
    {
        if (_isSolved)
            return;
        switch (_submissionState)
        {
            case SubmissionState.Unstarted:
                Debug.LogFormat("[I Forgor 💀 #{0}] This module wasn't even started when you submitted.", _id);
                GetComponent<KMBombModule>().HandleStrike();
                break;
            case SubmissionState.Strike:
                Debug.LogFormat("[I Forgor 💀 #{0}] This module wasn't even in the correct place when you submitted.", _id);
                GetComponent<KMBombModule>().HandleStrike();
                break;
            case SubmissionState.Left:
                if (v == 0)
                {
                    Debug.LogFormat("[I Forgor 💀 #{0}] Correct.", _id);
                    _isSolved = true;
                    GetComponent<KMBombModule>().HandlePass();
                }
                else
                {
                    Debug.LogFormat("[I Forgor 💀 #{0}] You pressed left when I wanted right.", _id);
                    GetComponent<KMBombModule>().HandleStrike();
                }
                break;
            case SubmissionState.Right:
                if (v == 1)
                {
                    _isSolved = true;
                    Debug.LogFormat("[I Forgor 💀 #{0}] Correct.", _id);
                    GetComponent<KMBombModule>().HandlePass();
                }
                else
                {
                    Debug.LogFormat("[I Forgor 💀 #{0}] You pressed right when I wanted left.", _id);
                    GetComponent<KMBombModule>().HandleStrike();
                }
                break;
        }
    }

    private enum SubmissionState
    {
        Unstarted,
        Strike,
        Left,
        Right
    }

    private IEnumerator WatchSolves()
    {
        //_prevSolved = _info.GetSolvedModuleNames().Count(s => !_ignored.Contains(s));
        _prevSolvedAll = _info.GetSolvedModuleNames().Count;

        while (true)
        {
            if (_prevSolvedAll != _info.GetSolvedModuleNames().Count)
            {
                ShuffleAll();
                yield return new WaitForSeconds(0.2f); // Be nice about "simultaneous" solves.
                _prevSolvedAll = _info.GetSolvedModuleNames().Count;
                //_prevSolved = _info.GetSolvedModuleNames().Count(s => !_ignored.Contains(s));
            }
            yield return null;
        }
    }

    private void ShuffleAll()
    {
        if (!_force)
        {
            List<string> all = _info.GetSolvableModuleNames();
            foreach (string n in _info.GetSolvedModuleNames())
                all.Remove(n);
            all.RemoveAll(n => _ignoredForForce.Contains(n));
            _force = all.Count == 0;
        }

        List<Transform> unassigned = _allAnchors.ToList();
        List<Component> unassignedChild = _allShufflable.ToList();

        foreach (Component c in _allShufflable)
            c.transform.SetParent(_currentParents[c], true);

        foreach (KeyValuePair<ForgorfulScript, Transform> kvp in _originalTransform)
        {
            Transform p;
            if (_force || UnityEngine.Random.value < 0.3f && unassigned.Contains(kvp.Value))
                p = kvp.Value;
            else
                p = unassigned.PickRandom();
            kvp.Key.transform.SetParent(p, false);
            unassigned.Remove(p);
            unassignedChild.Remove(_originalComponent[kvp.Key]);
            if (p.Equals(kvp.Value))
            {
                int num = ++_correctCounts[kvp.Key];
                Debug.LogFormat("[I Forgor 💀 #{0}] I am back home. My {1} eye would work right now.", kvp.Key._id, num % 2 == 0 ? "right" : "left");
                kvp.Key._submissionState = num % 2 == 1 ? SubmissionState.Right : SubmissionState.Left;
            }
            else
            {
                if (kvp.Key._submissionState == SubmissionState.Unstarted)
                    Debug.LogFormat("[I Forgor 💀 #{0}] I am now travelling.", kvp.Key._id);
                else if (kvp.Key._submissionState != SubmissionState.Strike)
                    Debug.LogFormat("[I Forgor 💀 #{0}] I am now travelling again.", kvp.Key._id);
                kvp.Key._submissionState = SubmissionState.Strike;
            }
        }

        if (_useTimer)
        {
            Transform tp = unassigned.PickRandom();
            _timer.transform.SetParent(tp, false);
            unassignedChild.Remove(_timer);
            unassigned.Remove(tp);
            _timerFace = _bombFaces[tp];
        }

        while (unassignedChild.Count != 0)
        {
            Transform p;
            if (_timerRequired.Contains(unassignedChild[0]))
            {
                var ap = unassigned.Where(tr => _bombFaces[tr] == _timerFace).ToList();
                if (ap.Count > 0)
                    p = ap.PickRandom();
                else
                    p = unassigned.PickRandom();
            }
            else
                p = unassigned.PickRandom();
            unassignedChild[0].transform.SetParent(p, false);
            unassignedChild.RemoveAt(0);
            unassigned.Remove(p);
        }

        foreach (Component c in _allShufflable)
        {
            _currentParents[c] = c.transform.parent;
            c.transform.SetParent(c.transform.parent.parent, true);
            Component sel;
            if (sel = c.GetComponent(_selType))
            {
                List<object> children = ((object[])_selChildren.GetValue(_selParent.GetValue(sel))).ToList();
                children.Remove(sel);
                Array r = (Array)Activator.CreateInstance(_selArrType, children.Count);
                children.ToArray().CopyTo(r, 0);
                _selDICSA.Invoke(_selParent.GetValue(sel), new object[0]);
                _selChildren.SetValue(_selParent.GetValue(sel), r);
                _selInit.Invoke(_selParent.GetValue(sel), new object[0]);
                _selParent.SetValue(sel, _bombFaces[_currentParents[c]]);
                children = ((object[])_selChildren.GetValue(_bombFaces[_currentParents[c]])).ToList();
                children.Add(sel);
                r = (Array)Activator.CreateInstance(_selArrType, children.Count);
                children.ToArray().CopyTo(r, 0);
                _selDICSA.Invoke(_bombFaces[_currentParents[c]], new object[0]);
                _selChildren.SetValue(_bombFaces[_currentParents[c]], r);
                _selInit.Invoke(_bombFaces[_currentParents[c]], new object[0]);
            }
        }

        MonoBehaviour h = (MonoBehaviour)FindObjectOfType(ReflectionHelper.FindTypeInGame("SelectableManager"));
        //h.GetType().MethodCall("LetGo", h, new object[0]);
        h.GetType().MethodCall("SelectRootDefault", h, new object[0]);
    }
}
