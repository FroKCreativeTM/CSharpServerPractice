using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufSize], 0, bufSize);    
        }

        // 얼마나 데이터가 쌓여있는가
        public int DataSize {  get { return _writePos - _readPos; } }

        // 버퍼에 남은 공간
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        // 현재 받은 데이터의 유효 범위
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        // 다음에 Recv를 할 때 어디부터 어디까지가 유효범위
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        // 버퍼가 찼을 때 다시 사용할 수 있게
        public void Clean()
        {
            int dataSize = DataSize;

            // 데이터를 전부 처리된 상태
            if(dataSize == 0)
            {
                // 인덱스 초기화
                _readPos = _writePos = 0;
            }
            // 데이터가 남은 경우
            else
            {
                // 남은 찌끄레기가 있으면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int nBytes)
        {
            // 처리한 값이 데이터 사이즈보다 큰 경우(비정상)
            if(nBytes >DataSize)
            {
                return false;
            }
            _readPos += nBytes;
            return true;
        }

        public bool OnWrite(int nBytes)
        {
            // 처리할 값이 남은 사이즈보다 큰 경우(비정상)
            if (nBytes > FreeSize)
            {
                return false;
            }
            _writePos += nBytes;
            return true;
        }
    }
}
