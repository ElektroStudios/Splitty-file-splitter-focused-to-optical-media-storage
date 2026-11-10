Imports System.Runtime.InteropServices

Friend Module NativeMethods

    <DllImport("dwmapi.dll", PreserveSig:=True)>
    Friend Function DwmSetWindowAttribute(hwnd As IntPtr,
                                          attr As Integer,
                                    ByRef attrValue As Integer,
                                          attrSize As Integer) As Integer
    End Function

End Module
