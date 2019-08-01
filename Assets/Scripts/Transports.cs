using System;
using UnityEngine;

enum TransportState
{
    ePaserHead,
    ePaserBody,
}

public class Transports 
{
    private const int s_MsgHeadLen = 4;

    private TransportState m_TransportState = TransportState.ePaserHead;
    //proto
    private byte[] m_MsgHead = new byte[s_MsgHeadLen];
    private byte[] m_MsgBody;
    private int m_iHadCopyHead;
    private int m_iHadCopyBody;


    private int m_iMsgID;
    private int m_iSeqID;
    private int m_iMsgBodyLen;


    public Action<int, byte[]> onDispatch = null;


    public byte[] PackBytes(int msgid, byte[] body)
    {
        byte[] buffer = new byte[body.Length + s_MsgHeadLen];
        buffer[0] = Convert.ToByte(msgid & 0xFF);
        buffer[1] = Convert.ToByte(msgid >> 8 & 0xFF);
        buffer[2] = Convert.ToByte(body.Length & 0xFF);
        buffer[3] = Convert.ToByte(body.Length >> 8 & 0xFF);

        WriteBytes(body, 0, buffer, 4, body.Length);

        return buffer;
    }
    public void ProBytes(byte[] buffer, int offset, int length)
    {
        if (buffer == null || length <= 0 || offset >= length) return;
        int iDataLen = length - offset;
        while (iDataLen > 0)
        {
            if (m_TransportState == TransportState.ePaserHead)
            {
                int iCopyHead = s_MsgHeadLen - m_iHadCopyHead;
                if (iDataLen >= iCopyHead)
                {
                    WriteBytes(buffer, offset, m_MsgHead, m_iHadCopyHead, iCopyHead);
                    offset += iCopyHead;
                    iDataLen -= iCopyHead;
                    m_iHadCopyHead = 0;

                    m_iMsgID = m_MsgHead[0] + (m_MsgHead[1] << 8);
                    m_iMsgBodyLen = m_MsgHead[2] + (m_MsgHead[3] << 8);

                    if (m_iMsgBodyLen > 0)
                    {
                        m_iHadCopyBody = 0;
                        m_MsgBody = new byte[m_iMsgBodyLen];
                        m_TransportState = TransportState.ePaserBody;
                    }
                    else
                    {
                        if (onDispatch != null)
                        {
                            onDispatch(m_iMsgID, m_MsgBody);
                        }
                        ResetPaserState();
                    }
                }
                else
                {
                    WriteBytes(buffer, offset, m_MsgHead, m_iHadCopyHead, iDataLen);
                    m_iHadCopyHead += iDataLen;
                    return;
                }
            }
            else if (m_TransportState == TransportState.ePaserBody)
            {
                int iCopyBody = m_iMsgBodyLen - m_iHadCopyBody;
                if (iDataLen >= iCopyBody)
                {
                    WriteBytes(buffer, offset, m_MsgBody, m_iHadCopyBody, iCopyBody);
                    offset += iCopyBody;
                    iDataLen -= iCopyBody;
                    if (onDispatch != null)
                    {
                        onDispatch(m_iMsgID, m_MsgBody);
                    }
                    ResetPaserState();
                }
                else
                {
                    WriteBytes(buffer, offset, m_MsgBody, m_iHadCopyBody, iDataLen);
                    m_iHadCopyBody += iDataLen;
                    return;
                }

            }
        }
    }

    private void ResetPaserState()
    {
        m_TransportState = TransportState.ePaserHead;
        m_iHadCopyBody = 0;
        m_iHadCopyHead = 0;
    }

    private void WriteBytes(byte[] src, int srcOffset, byte[] dst, int dstOffset, int length)
    {
        Buffer.BlockCopy(src, srcOffset, dst, dstOffset, length);
    }
}