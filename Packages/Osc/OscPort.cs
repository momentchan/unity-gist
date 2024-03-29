using System.Net.Sockets;
using System;
using System.Net;
using Osc;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Threading;
using System.Linq;

namespace Osc {
	public abstract class OscPort : MonoBehaviour {
		public enum ReceiveModeEnum { Event = 0, Poll }

		public const int BUFFER_SIZE = 1 << 16;
		public List<ReceiveEvent> OnReceives;
		public ExceptionEvent OnError;

		public ReceiveModeEnum receiveMode = ReceiveModeEnum.Event;

		[SerializeField] protected int localPort = 0;
		[SerializeField] protected string defaultRemoteHost = "localhost";
		[SerializeField] protected int defaultRemotePort = 10000;
		[SerializeField] protected int limitReceiveBuffer = 10;

		public virtual bool IsSendToLocal { get; set; } = false;

		protected Parser _oscParser;
		protected Queue<Capsule> _received;
		protected Queue<System.Exception> _errors;
		protected IPEndPoint _defaultRemote;
		protected IPEndPoint _local;

		public virtual IEnumerable<Capsule> PollReceived() {
			lock (_received) {
				while (_received.Count > 0)
					yield return _received.Dequeue ();
			}
		}
		public virtual IEnumerable<System.Exception> PollException() {
			lock (_errors) {
				while (_errors.Count > 0)
					yield return _errors.Dequeue ();
			}
		}

		public void Send(MessageEncoder oscMessage) {
			Send (oscMessage, _defaultRemote);
		}
		public void Send(MessageEncoder oscMessage, IPEndPoint remote) {
			Send (oscMessage.Encode (), remote);
		}
		public void Send(byte[] oscData) {
			Send (oscData, _defaultRemote);
		}

		public void SendToLocal(byte[] oscData)
		{
			Send(oscData, _local);
		}

		public abstract void Send (byte[] oscData, IPEndPoint remote);

		public IPAddress FindFromHostName(string hostname) {
			var addresses = Dns.GetHostAddresses (hostname);
			IPAddress address = IPAddress.None;
			for (var i = 0; i < addresses.Length; i++) {
				if (addresses[i].AddressFamily == AddressFamily.InterNetwork) {
					address = addresses[i];
					break;
				}
			}
			return address;
		}
        public void UpdateDefaultRemote () {
            _defaultRemote = new IPEndPoint (FindFromHostName (defaultRemoteHost), defaultRemotePort);
			_local = new IPEndPoint (FindFromHostName ("localhost"), defaultRemotePort);
        }

		protected virtual void OnEnable() {
			_oscParser = new Parser ();
			_received = new Queue<Capsule> ();
			_errors = new Queue<Exception> ();
			UpdateDefaultRemote();
		}
		protected virtual void OnDisable() {
		}

		protected virtual void Update() {
			if (receiveMode == ReceiveModeEnum.Event) {
				lock (_received)
					while (_received.Count > 0)
					{
						var capsule = _received.Dequeue();
						var receive = OnReceives.FirstOrDefault(r => r.path == capsule.message.path);
						if(receive!=null)
							receive.onReceived.Invoke(capsule);
					}
				lock (_errors)
					while (_errors.Count > 0)
						OnError.Invoke (_errors.Dequeue ());
			}
		}
		protected void RaiseError(System.Exception e) {
			_errors.Enqueue (e);
		}
		protected void Receive(OscPort.Capsule c) {
			if (limitReceiveBuffer <= 0 || _received.Count < limitReceiveBuffer)
				_received.Enqueue (c);
		}

		#region classes
		public struct Capsule {
			public Message message;
			public IPEndPoint ip;

			public Capsule(Message message, IPEndPoint ip) {
				this.message = message;
				this.ip = ip;
			}
		}
		#endregion
	}

	[System.Serializable]
	public class ExceptionEvent : UnityEvent<Exception> {}
	[System.Serializable]
	public class ReceiveEvent
    {
		public string path;
		public CapsuleEvent onReceived;
    }
	[System.Serializable]
	public class CapsuleEvent : UnityEvent<OscPort.Capsule> {}
	[System.Serializable]
	public class MessageEvent : UnityEvent<Message> {}
}