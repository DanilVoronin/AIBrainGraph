using UnityEngine;

namespace Singletons
{
	/// <summary>
	/// Persistent singleton.
	/// </summary>
	public class MMPersistentSingleton<T> : MonoBehaviour	where T : Component
	{
		[Header("Persistent Singleton")]
		/// if this is true, this singleton will auto detach if it finds itself parented on awake
		[Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
		public bool AutomaticallyUnparentOnAwake = true;
		
		public static bool HasInstance => _instance != null;
		public static T Current => _instance;
		
		protected static T _instance;
		protected bool _enabled;

		/// <summary>
		/// Singleton design pattern
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T> ();
						_instance.SendMessage("SetInit");
                    }
				}
				return _instance;
			}
		}

		/// <summary>
		/// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
		/// </summary>
		protected virtual void Awake ()
		{
			SetInit();

        }

		/// <summary>
		/// ѕозвол€ет инициализировать вне класса
		/// </summary>
		public void SetInit()
		{
            if (!_enabled)
            {
                InitializeSingleton();
            }
        }

        /// <summary>
        /// Initializes the singleton.
        /// </summary>
        protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (AutomaticallyUnparentOnAwake)
			{
				this.transform.SetParent(null);
			}

			if(_instance == null) _instance = this as T;

            if (this != _instance)
			{
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad (transform.gameObject);
            _enabled = true;
        }

		protected virtual void OnDestroy()
		{
			if (this == _instance)
			{
                _instance = null;
            }                
        }
	}
}