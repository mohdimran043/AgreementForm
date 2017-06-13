Imports System.Runtime.InteropServices

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Me.ShowInTaskbar = False
        FormBorderStyle = FormBorderStyle.None
        WindowState = FormWindowState.Maximized
        TableLayoutPanel1.Width = Me.Width
        TableLayoutPanel1.Height = Me.Height
        WebBrowser1.Width = Me.Width
        TableLayoutPanel2.Width = Me.Width
        'KeyboardHook(Me, e)
        ' EnableWindow(1, False)
        'Me.ShowInTaskbar = True
        ' MyFilter.SuppressKeys = Not MyFilter.SuppressKeys

    End Sub

    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        Return False
    End Function
    Private Delegate Function LowLevelKeyboardProcDelegate(nCode As Integer, wParam As Integer, ByRef lParam As KBDLLHOOKSTRUCT) As Integer

    <DllImport("user32.dll", EntryPoint:="SetWindowsHookExA", CharSet:=CharSet.Ansi)> _
    Private Shared Function SetWindowsHookEx(idHook As Integer, lpfn As LowLevelKeyboardProcDelegate, hMod As Integer, dwThreadId As Integer) As Integer
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function UnhookWindowsHookEx(hHook As Integer) As Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="CallNextHookEx", CharSet:=CharSet.Ansi)> _
    Private Shared Function CallNextHookEx(hHook As Integer, nCode As Integer, wParam As Integer, ByRef lParam As KBDLLHOOKSTRUCT) As Integer
    End Function

    Const WH_KEYBOARD_LL As Integer = 13
    Private intLLKey As Integer
    Private lParam As KBDLLHOOKSTRUCT

    Private Structure KBDLLHOOKSTRUCT
        Public vkCode As Integer
        Private scanCode As Integer
        Public flags As Integer
        Private time As Integer
        Private dwExtraInfo As Integer
    End Structure
    <DllImport("user32.dll")> _
    Private Shared Function EnableWindow(hWnd As IntPtr, enable As Boolean) As Boolean
    End Function
    Private Function LowLevelKeyboardProc(nCode As Integer, wParam As Integer, ByRef lParam As KBDLLHOOKSTRUCT) As Integer
        Dim blnEat As Boolean = False
        Select Case wParam
            Case 256, 257, 260, 261
                'Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key
                If ((lParam.vkCode = 9) AndAlso (lParam.flags = 32)) OrElse ((lParam.vkCode = 27) AndAlso (lParam.flags = 32)) OrElse ((lParam.vkCode = 27) AndAlso (lParam.flags = 0)) OrElse ((lParam.vkCode = 91) AndAlso (lParam.flags = 1)) OrElse ((lParam.vkCode = 92) AndAlso (lParam.flags = 1)) OrElse ((True) AndAlso (lParam.flags = 32)) Then
                    blnEat = True
                End If
                Exit Select
        End Select

        If blnEat Then
            Return 1
        Else
            Return CallNextHookEx(0, nCode, wParam, lParam)
        End If

    End Function

    Private Sub KeyboardHook(sender As Object, e As EventArgs)
        intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, New LowLevelKeyboardProcDelegate(AddressOf LowLevelKeyboardProc), System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()(0)).ToInt32(), 0)
    End Sub

    Private Sub ReleaseKeyboardHook()
        intLLKey = UnhookWindowsHookEx(intLLKey)
    End Sub

    Private Sub webBrowser1_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        KeyboardHook(Me, e)
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub TableLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel1.Paint

    End Sub
End Class

Public Class MyFilter
    Implements IMessageFilter

    Public Shared SuppressKeys As Boolean = False

    Private Const WM_KEYDOWN As Integer = &H100
    Private Const WM_KEYUP As Integer = &H101

    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
        Select Case m.Msg
            Case WM_KEYDOWN, WM_KEYUP
                If MyFilter.SuppressKeys Then
                    Return True
                End If
                Exit Select
            Case Else

                Exit Select
        End Select
        Return False
    End Function

End Class

Partial Public Class NativeMethods

    ''' Return Type: BOOL->int
    '''fBlockIt: BOOL->int
    <System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint:="BlockInput")> _
    Public Shared Function BlockInput(<System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)> fBlockIt As Boolean) As <System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)> Boolean
    End Function

End Class
