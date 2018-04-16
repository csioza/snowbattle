using System;
using System.Collections.Generic;
using System.Text;

namespace NGE.Network {
	public class NetworkErrorReporter {
		private NetworkErrorReporter() { }
		private static NetworkErrorReporter _instance = new NetworkErrorReporter();
		public static NetworkErrorReporter Instance { get { return _instance; } }
		public int nSocketAsyncConnectFailed;
        public int nAsyncSendFailure_SocketException;
		public void PrintConsole() {
			Console.WriteLine("nSocketAsyncConnectFailed : " + nSocketAsyncConnectFailed);
            Console.WriteLine("nSocketAsyncSendFailed : " + nAsyncSendFailure_SocketException);
		}
	}
	public class NetworkPerformanceStatistics {
		private NetworkPerformanceStatistics() { }
		private static NetworkPerformanceStatistics _instance = new NetworkPerformanceStatistics();
		public static NetworkPerformanceStatistics Instance { get { return _instance; } }

		public int nAsyncSocketAcceptBegin;
		public int nAsyncSocketAcceptEnd;
		public int nAsyncSocketConnectBegin;
		public int nAsyncSocketConnectEnd;
		public int nAsyncRecvBegin;
		public int nAsyncRecvEnd;
		public int nAsyncSendBegin;
		public int nAsyncSendEnd;
		public int nPacketCachePushed;
		public int nPacketCachePoped;
		public int nCacheBlock;

		//public int nAsyncBlockSendBegin;
		//public int nAsyncPacketSendReq;
		//public int nAsyncPacketSendBegin;
		//public int nAsyncPacketSendEnd;
		//public int nCacheBlockSendBegin

		public int nSendPacketSizeMax=-1;
		public int nSendPacketSizeMin= 0x7fffffff;
		public int nRecvPacketSizeMax = -1;
		public int nRecvPacketSizeMin = 0x7fffffff;


		public void PrintConsole() {
			Console.WriteLine("nAsyncSocketAccept[Begin/End] : {0}/{1}" ,nAsyncSocketAcceptBegin   ,nAsyncSocketAcceptEnd );
			Console.WriteLine("nAsyncSocketConnect[Begin/End] : {0}/{1}", nAsyncSocketConnectBegin, nAsyncSocketConnectEnd);
 			Console.WriteLine("nAsyncRecv[Begin/End] : {0}/{1}" , nAsyncRecvBegin,nAsyncRecvEnd);
			Console.WriteLine("nAsyncSend[Begin/End] : {0}/{1}" , nAsyncSendBegin,nAsyncSendEnd);
			//Console.WriteLine("nPacketCache[Pushed/Popuped/CurBlock] : {0}/{1}/{2}",nPacketCachePushed , nPacketCachePoped,nCacheBlock);
			Console.WriteLine("nSendPacketSize[Min/Max] : {0}/{1}", nSendPacketSizeMin,nSendPacketSizeMax );
			Console.WriteLine("nRecvPacketSize[Min/Max] : {0}/{1}", nRecvPacketSizeMin, nRecvPacketSizeMax);

		}
	}

}
