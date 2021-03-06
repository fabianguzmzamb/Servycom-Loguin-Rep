﻿
Imports System.Data.OleDb
Public Class rrhh
    Dim CONECTOR As New OleDbConnection(My.Settings.CADENA)
    Dim COMANDO As New OleDbCommand
    Dim adaptador As New OleDbDataAdapter(COMANDO)
    Dim TABLA As New DataTable
    Dim ds As DataSet

    Private Sub rrhh_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cargar_grilla_empleados()
        Dim mes As Integer

        cumples.Items.Clear()
        mes = CStr(dateHoy.Value.Month)
        Dim dr3 As OleDbDataReader
        Dim comandoo As New OleDbCommand
        comandoo = New OleDbCommand("select * from empledos where mes = '" & mes & "'", CONECTOR)
        CONECTOR.Open()
        dr3 = comandoo.ExecuteReader()
        If dr3.HasRows Then
            While dr3.Read

                lblCumple.Visible = True
                torta.Visible = True
                cumples.Items.Add(dr3("nombre"))
            End While
        End If
        CONECTOR.Close()



        radioSi.Checked = True
    End Sub
    Sub cargar_grilla_empleados()
        ''procedimiento q carga la grilla con la nomina de empleados y pinta de rojo los legajos dados de baja
        Dim hoy As Date
        hoy = txtFechaLlamado.Value.Date

        Dim ADAPTADOR As New OleDbDataAdapter(COMANDO)
        COMANDO.Connection = CONECTOR
        COMANDO.CommandType = CommandType.TableDirect
        COMANDO.CommandText = "empledos"
        ADAPTADOR.Fill(TABLA)
        ''esto agregue
        llenar_combo_empleados_premios(comboFiltroEmpleados)
        llenar_combo_empleados_premios(comboEmpleadoLlamado)
        llenar_combo_empleados_premios(comboCapaEmpleado)
        llenar_combo_empleados_premios(comboEmpleadoNuevaCapa)
        llenar_combo_empleados_premios(comboEmplEntregasFiltro)
        llenar_combo_empleados_premios(comboEmpleadosEntregas)
        grillaEmpleados.DataSource = TABLA

        comboFiltroEmpleados.DisplayMember = "nombre"
        comboFiltroEmpleados.ValueMember = "dni"
        comboFiltroEmpleados.DataSource = TABLA
        llenar_combo_capacitaciones()
        For Each Row As DataGridViewRow In grillaEmpleados.Rows
            If Row.Cells("estado").Value = "Baja" Then
                Row.DefaultCellStyle.BackColor = Color.Red
            Else
                If CDate(Row.Cells("apto").Value) <= hoy Or CDate(Row.Cells("carnet").Value) <= hoy Then
                    Row.DefaultCellStyle.BackColor = Color.Yellow
                Else
                    Row.DefaultCellStyle.BackColor = Color.Green
            End If
            End If
        Next
    End Sub

    Public Sub llenar_combo_empleados_premios(combo As ComboBox)
        ''llenar combo por nombre de empleados para filtrar los llamados de atencion
        combo.DisplayMember = "nombre"
        combo.ValueMember = "dni"
        combo.DataSource = TABLA
    End Sub
    Sub llenar_combo_capacitaciones()
        ''procedimiento q llena el combo por titulo de capacitacion
        Dim tabli As New DataTable
        Dim ADAPTADOR As New OleDbDataAdapter(COMANDO)
        COMANDO.Connection = CONECTOR
        COMANDO.CommandType = CommandType.TableDirect
        COMANDO.CommandText = "capacitaciones"
        ADAPTADOR.Fill(tabli)
        grillaCapa.DataSource = tabli
        comboCapacitaciones.DataSource = tabli
        comboCapacitaciones.DisplayMember = "titulo"

    End Sub

    Private Sub btnInsertarLlamado_Click(sender As Object, e As EventArgs) Handles btnInsertarLlamado.Click
        insertarDatos()
    End Sub
    Sub insertarDatos()
        ''insertar llamados de atencion en tabla llamado
        Dim empleado As Integer
        Dim descripcion As String
        Dim fecha As String
        Dim tipo As String

        empleado = comboEmpleadoLlamado.SelectedValue

        descripcion = txtDetalleLlamado.Text
        fecha = txtFechaLlamado.Value.Date
        tipo = comboTipoLlamado.Text
        CONECTOR.Open()
        Dim comando As New OleDbCommand("insert into llamado values( " & empleado & ",'" & tipo & "','" & fecha & "','" & descripcion & "')", CONECTOR)
        comando.ExecuteNonQuery()
        CONECTOR.Close()
        txtDetalleLlamado.Text = ""
    End Sub

    Private Sub btnBuscarEmpleadoLlamado_Click(sender As Object, e As EventArgs) Handles btnBuscarEmpleadoLlamado.Click
        '' llenar grilla con llamados de atencion de empleado filtrado en combo
        grillaLlamado.Rows.Clear()
        Dim empleado As Integer
        Dim dr As OleDbDataReader
        empleado = comboEmpleadoLlamado.SelectedValue
        COMANDO = New OleDbCommand("select * from llamado where dni=" & empleado, CONECTOR)
        CONECTOR.Open()
        dr = COMANDO.ExecuteReader()

        If dr.HasRows Then
            While dr.Read
                grillaLlamado.Rows.Add(dr("dni"), dr("tipo"), dr("fecha"), dr("detalle"))
            End While
        Else
            MsgBox("No hay registros para " & comboEmpleadoLlamado.SelectedItem)
        End If
        CONECTOR.Close()
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        NuevoLegajo.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ModificarLegajo.Show()
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnFiltrarXempleado.Click
        ''cargar grilla con capcitaciones segun empleado elegido en combo

        ''grillaCapa.Rows.Clear()
        Dim empleado As Integer
        Dim dr As OleDbDataReader
        empleado = comboCapaEmpleado.SelectedValue
        COMANDO = New OleDbCommand("select * from capacitaciones where dni=" & empleado, CONECTOR)
        CONECTOR.Open()
        dr = COMANDO.ExecuteReader()

        If dr.HasRows Then
            While dr.Read
                grillaCapa.Rows.Add(comboCapaEmpleado.Text, dr("titulo"), dr("certificado"), dr("vencimiento"))
            End While
        End If
        CONECTOR.Close()

    End Sub

    Private Sub btnFiltrarXcapacitacion_Click(sender As Object, e As EventArgs) Handles btnFiltrarXcapacitacion.Click
        ''cargar grilla con capacitaciones segun capa elegida en combo
        grillaCapa.Rows.Clear()
        Dim capacitacion As String
        Dim dr As OleDbDataReader
        capacitacion = comboCapacitaciones.Text
        COMANDO = New OleDbCommand("select * from capacitaciones where titulo='" & capacitacion & "'", CONECTOR)
        CONECTOR.Open()
        dr = COMANDO.ExecuteReader()
        If dr.HasRows Then
            While dr.Read
                grillaCapa.Rows.Add(dr("dni"), dr("titulo"), dr("certificado"), dr("vencimiento"))
            End While
        End If
        CONECTOR.Close()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ''insertar capacitacion nueva
        grillaCapa.Rows.Clear()
        Dim certificacion As String
        If radioSi.Checked Then
            certificacion = "SI"
        Else
            certificacion = "NO"
        End If
        CONECTOR.Open()
        Dim comando As New OleDbCommand("insert into capacitaciones values( " & comboEmpleadoNuevaCapa.SelectedValue & ",'" & comboEmpleadoNuevaCapa.Text & "','" & txtTituloNuevaCapa.Text & "','" & certificacion & "','" & txtVnecimientoCapaNeva.Text & "')", CONECTOR)
        comando.ExecuteNonQuery()

        ''actualizar grilla
        Dim dr As OleDbDataReader
        Dim COMAND = New OleDbCommand("select * from capacitaciones ", CONECTOR)

        dr = COMAND.ExecuteReader()
        If dr.HasRows Then
            While dr.Read
                grillaCapa.Rows.Add(dr("nombre"), dr("titulo"), dr("certificado"), dr("vencimiento"))
            End While
        End If
        CONECTOR.Close()
        txtTituloNuevaCapa.Text = ""
        txtVnecimientoCapaNeva.Text = ""
        llenar_combo_capacitaciones()
    End Sub

    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
        ''Insertar los elementos entregados en tabla entregas
        Dim remera As String
        Dim pantalon As String
        Dim camisa As String
        Dim campera As String
        Dim buzo As String
        Dim zapatos As String
        If radioBuzo.Checked Then
            buzo = "SI"
        Else
            buzo = "NO"
        End If

        If radioCamisa.Checked Then
            camisa = "SI"
        Else
            camisa = "NO"
        End If

        If radioCampera.Checked Then
            campera = "SI"
        Else
            campera = "NO"
        End If

        If radioPantalon.Checked Then
            pantalon = "SI"
        Else
            pantalon = "NO"
        End If

        If radioRemera.Checked Then
            remera = "SI"
        Else
            remera = "NO"
        End If

        If radioZapatos.Checked Then
            zapatos = "SI"
        Else
            zapatos = "NO"
        End If
        CONECTOR.Open()
        Dim comando As New OleDbCommand("insert into entregas values( " & comboEmpleadosEntregas.SelectedValue & ",'" & comboEmpleadosEntregas.Text & "','" & txtFechaEntregas.Value.Date & "','" & remera & "','" & pantalon & "','" & camisa & "','" & campera & "','" & buzo & "','" & zapatos & "')", CONECTOR)
        comando.ExecuteNonQuery()

        ''actualizar grilla
        Dim dr As OleDbDataReader
        Dim COMAND = New OleDbCommand("select * from entregas ", CONECTOR)

        dr = COMAND.ExecuteReader()
        If dr.HasRows Then
            While dr.Read
                grillaEntregas.Rows.Add(dr("nombre"), dr("remera"), dr("pantalon"), dr("camisa"), dr("campera"), dr("buzo"), dr("zapatos"), dr("fecha"))
            End While
        End If
        CONECTOR.Close()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        grillaEntregas.Rows.Clear()
        ''cargar grilla con capacitaciones segun capa elegida en combo
        Dim empleado As String
        Dim dr As OleDbDataReader
        empleado = comboEmplEntregasFiltro.Text
        COMANDO = New OleDbCommand("select * from entregas where nombre='" & empleado & "'", CONECTOR)
        CONECTOR.Open()
        dr = COMANDO.ExecuteReader()
        If dr.HasRows Then
            While dr.Read
                grillaEntregas.Rows.Add(dr("nombre"), dr("remera"), dr("pantalon"), dr("camisa"), dr("campera"), dr("buzo"), dr("zapatos"), dr("fecha"))
            End While
        End If
        CONECTOR.Close()
    End Sub


End Class