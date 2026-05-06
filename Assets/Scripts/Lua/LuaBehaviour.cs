using System;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour
{
    public TextAsset luaScript;

    internal static LuaEnv luaEnv = new LuaEnv();
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptLuaTable;

    private void Awake()
    {
        scriptLuaTable = luaEnv.NewTable();

        using(LuaTable meta = luaEnv.NewTable())
        {
            meta.Set("__index", luaEnv.Global);
            scriptLuaTable.SetMetaTable(meta);
        }

        scriptLuaTable.Set("self", this);

        // 执行lua脚本
        luaEnv.DoString(luaScript.text, luaScript.name, scriptLuaTable);

        // 从lua脚本中获取对应函数
        Action luaAwake = scriptLuaTable.Get<Action>("awake");
        scriptLuaTable.Get("start", out luaStart);
        scriptLuaTable.Get("update", out luaUpdate);
        scriptLuaTable.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
            luaAwake();
    }

    private void Start()
    {
        if(luaStart != null)
            luaStart();
    }

    private void Update()
    {
        if(luaUpdate != null)
            luaUpdate();

        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
    }

    private void OnDestroy()
    {
        if (luaOnDestroy != null)
            luaOnDestroy();

        scriptLuaTable.Dispose();
        luaStart = null;
        luaUpdate = null;
        luaOnDestroy = null;
    }
}
