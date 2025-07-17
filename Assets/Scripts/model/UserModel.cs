using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZhiSe;

/// <summary>
/// 用户数据
/// </summary>
internal class UserModel
{

	private static UserModel instance;

	public static UserModel Ins
	{
		get
		{
			instance ??= new UserModel();
			return instance;
		}
	}
	// 是否是新用户
	private bool _isNew = true;
	private long _loginTime = 0;
	// 当前金币	
	private int _coin = 0;
	public int coin
	{
		get { return _coin; }
		set { _coin = value; SetUserData("coin", _coin); }
	}

	// 当前关卡
	private int _level = 1;
	public int level
	{
		get
		{
			return 1;//_level;
		}
		set
		{
			_level = value;
			SetUserData("level", _level);
		}
	}
	// 用户ID
	public string userId;
	// 是否是测试号
	public bool isTest = false;
	public string version;

	private UserModel()
	{
		instance = this;
	}

	public void InitData(Dictionary<string, object> ret)
	{
		DataParse(ret, this);

		long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		long preTime = _loginTime;
		_loginTime = currentTimestamp;
		Dictionary<string, object> data = new Dictionary<string, object>();
		bool isNewDay = _loginTime / 86400 > preTime / 86400;
		if (_isNew)
		{
			_isNew = false;
			data.Add("loginTime", _loginTime);
			data.Add("isNew", _isNew);
		}
		else
		{
			data.Add("loginTime", _loginTime);
		}

		if (isNewDay)
		{

		}
		SetUserData(data);
	}

	private void DataParse<T>(Dictionary<string, object> data, T target)
	{
		var type = typeof(T);
		var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

		foreach (var item in data)
		{
			string key = item.Key;
			object value = item.Value;
			var field = fields.FirstOrDefault(f => f.Name.Equals($"_{key}", StringComparison.OrdinalIgnoreCase));

			if (field != null && value != null)
			{
				try
				{
					// 转换为目标字段类型
					object convertedValue = Convert.ChangeType(value, field.FieldType);
					field.SetValue(target, convertedValue);
				}
				catch { /* 异常处理 */ }
			}
		}
	}

	private void SetUserData<T>(string key, T value)
	{

		var hashData = new Hashtable();
		key = "user_" + key;
		Dictionary<string, object> data = new() {
			{ key, value }
		};

		hashData = new(){
			{ key, value }
		};
		if (!GameData.isLocal)
		{
			Seeg.SetUserData(data);
		}
		DataManager.Instance.SetData(hashData);
	}

	private void SetUserData(Dictionary<string, object> keyValuePairs)
	{
		var data = new Dictionary<string, object>();
		var hashData = new Hashtable();
		foreach (var pair in keyValuePairs)
		{
			data["user_" + pair.Key] = pair.Value;
			hashData["user_" + pair.Key] = pair.Value;
		}

		if (!GameData.isLocal)
		{
			Seeg.SetUserData(data);
		}
		DataManager.Instance.SetData(hashData);
	}

	private void SetDotData(Dictionary<string, object> keyValuePairs)
	{
		var data = new Dictionary<string, object>();
		var hashData = new Hashtable();
		foreach (var pair in keyValuePairs)
		{
			data["dot_" + pair.Key] = pair.Value;
			hashData["dot_" + pair.Key] = pair.Value;
		}
		if (!GameData.isLocal)
		{
			Seeg.SetUserData(data);
		}
		DataManager.Instance.SetData(hashData);
	}
}