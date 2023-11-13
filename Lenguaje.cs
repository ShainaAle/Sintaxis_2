using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/*
    Requerimiento 1: Programar scanf 
    Requerimiento 2: Programar printf
    Requerimiento 3: Programar ++,--,+=,-=,*=,/=,%=
    Requerimiento 4: Programar else
    Requerimiento 5: Programar do para que genere una sola vez el codigo
    Requerimiento 6: Programar while para que genere una sola vez el codigo
    Requerimiento 7: Programar el for para que generere una sola vez el codigo
    Requerimiento 8: Programar el CAST
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        int contadorIf, contadorFor;
        int contadorDo;
        int contadorWhile;
        int contadorElse;
        Variable.TiposDatos tipoDatoExpresion;

        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contadorIf = contadorFor = contadorDo = contadorWhile = contadorElse = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
            contadorIf = contadorFor = contadorDo = contadorWhile = contadorElse = 1;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("org 100h");
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main(true);
            asm.WriteLine("int 20h");
            asm.WriteLine("RET");
            asm.WriteLine("define_scan_num");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            Imprime();
            asm.WriteLine("END");
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            asm.WriteLine("; V a r i a b l e s");
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDatos() + " = " + v.getValor());
                asm.WriteLine(v.getNombre() + " dw 0h");
            }
            log.WriteLine("-----------------");
        }

        private bool Existe(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        private void Modifica(string nombre, float nuevoValor)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TiposDatos getTipo(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getTiposDatos();
                }
            }
            return Variable.TiposDatos.Char;
        }
        private Variable.TiposDatos getTipo(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TiposDatos.Float;
            }
            else if (resultado < 256)
            {
                return Variable.TiposDatos.Char;
            }
            else if (resultado < 65536)
            {
                return Variable.TiposDatos.Int;
            }
            return Variable.TiposDatos.Float;
        }
        // Libreria -> #include<Identificador(.h)?>
        private void Libreria()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                match("h");
            }
            match(">");
        }
        //Librerias -> Libreria Librerias?
        private void Librerias()
        {
            Libreria();
            if (getContenido() == "#")
            {
                Librerias();
            }
        }

        //Variables -> tipo_dato ListaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TiposDatos tipo = Variable.TiposDatos.Char;

            switch (getContenido())
            {
                case "int": tipo = Variable.TiposDatos.Int; break;
                case "float": tipo = Variable.TiposDatos.Float; break;
            }

            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");

            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TiposDatos tipo)
        {
            if (!Existe(getContenido()))
            {
                lista.Add(new Variable(getContenido(), tipo));
            }

            else
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> está duplicada", log, linea, columna);
            }

            match(Tipos.Identificador);

            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(tipo);
            }

        }
        //BloqueInstrucciones -> { ListaInstrucciones ? }
        private void BloqueInstrucciones(bool ejecuta, bool primeraVez)
        {
            match("{");

            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta, primeraVez);
            }

            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta, bool primeraVez)
        {
            Instruccion(ejecuta, primeraVez);

            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta, primeraVez);
            }

        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta, bool primeraVez)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta, primeraVez);
            }

            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta, primeraVez);
            }

            else if (getContenido() == "if")
            {
                If(ejecuta, primeraVez);
            }

            else if (getContenido() == "while")
            {
                While(ejecuta, primeraVez);
            }

            else if (getContenido() == "do")
            {
                Do(ejecuta, primeraVez);
            }

            else if (getContenido() == "for")
            {
                For(ejecuta, primeraVez);
            }

            else
            {
                Asignacion(ejecuta, primeraVez);
            }
        }

        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta, bool primeraVez)
        {
            float resultado = 0;
            tipoDatoExpresion = Variable.TiposDatos.Char;

            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            log.Write(getContenido() + " = ");
            string var = getContenido();
            match(Tipos.Identificador);

            if (getContenido() == "=")
            {
                match("=");
                Expresion(primeraVez);
                resultado = stack.Pop();

                if (primeraVez)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine(";Asignacion " + var);
                    asm.WriteLine("MOV " + var + ", AX");
                }
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    resultado = getValor(var) + 1;

                    if (primeraVez)
                    {
                        asm.WriteLine("INC  " + var);
                    }
                }
                else
                {
                    match("--");
                    resultado = getValor(var) - 1;

                    if (primeraVez)
                    {
                        asm.WriteLine("DEC  " + var);
                    }
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {
                resultado = getValor(var);
                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion(primeraVez);
                    resultado += stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("ADD  " + var + " ,AX");
                    }
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion(primeraVez);
                    resultado -= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("SUB " + var + " ,AX");
                    }
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion(primeraVez);
                    resultado *= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP BX");
                        asm.WriteLine("MOV BX," + var);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("MOV " + var + " ,AX");
                    }
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion(primeraVez);
                    resultado /= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP BX");
                        asm.WriteLine("MOV BX," + var);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("MOV " + var + " ,AX");
                    }
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion(primeraVez);
                    resultado %= stack.Pop();

                    if (primeraVez)
                    {
                        asm.WriteLine("POP BX");
                        asm.WriteLine("MOV AX" + var);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("MOV " + var + " ,DX");
                    }
                }
            }
            log.WriteLine(" = " + resultado);

            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo(var);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                if (tipoDatoExpresion > tipoDatoResultado)
                {
                    tipoDatoResultado = tipoDatoExpresion;
                }

                if (tipoDatoVariable >= tipoDatoResultado)
                {
                    Modifica(var, resultado);
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                }
            }
            match(";");
        }

        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta, bool primeraVez)
        {
            asm.WriteLine("; While: " + contadorWhile);
            string etiquetaIniciowhile = "InicioWhile" + contadorWhile;
            string etiquetaFinwhile = "FinWhile" + contadorWhile++;
            int inicia = character;
            int lineaInicio = linea;
            string var = getContenido();

            log.WriteLine("While: " + var);

            if (primeraVez)
            {
                asm.WriteLine(etiquetaIniciowhile + ":");
            }

            do
            {
                match("while");
                match("(");
                ejecuta = Condicion(etiquetaFinwhile, primeraVez) && ejecuta;
                match(")");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }

                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    character = inicia - var.Length - 1;
                    archivo.BaseStream.Seek(character, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }

                if (primeraVez)
                {
                    asm.WriteLine("JMP " + etiquetaIniciowhile);
                    asm.WriteLine(etiquetaFinwhile + ":");
                }
                primeraVez = false;
            }

            while (ejecuta);
        }

        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta, bool primeraVez)
        {

            string etiquetaIniciodo = "InicioDo" + contadorDo;
            string etiquetaFindo = "FinDo" + contadorDo++;
            int inicia = character;
            int lineaInicho = linea;
            string variable = getContenido();
            log.WriteLine("Do: ");

            if (primeraVez)
            {
                asm.WriteLine(etiquetaIniciodo + ":");
            }

            match("do");

            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }

                match("while");
                match("(");
                ejecuta = Condicion(etiquetaFindo, primeraVez) && ejecuta;
                match(")");
                match(";");

                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    character = inicia;
                    archivo.BaseStream.Seek(character, SeekOrigin.Begin);
                    nextToken();
                    lineaInicho = linea;
                }

                if (primeraVez)
                {
                    asm.WriteLine("JMP " + etiquetaIniciodo);
                    asm.WriteLine(etiquetaFindo + ":");

                }

                primeraVez = false;
            }
            while (ejecuta);
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta, bool primeraVez)
        {
            asm.WriteLine("; For: " + contadorFor);
            match("for");
            match("(");
            Asignacion(ejecuta, primeraVez);
            string etiquetaInicio = "InicioFor" + contadorFor;
            string etiquetaFin = "FinFor" + contadorFor++;
            int inicia = character;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();
            log.WriteLine("For: " + variable);

            if (primeraVez)
            {
                asm.WriteLine(etiquetaInicio + ":");
            }

            do
            {
                ejecuta = Condicion(etiquetaFin, primeraVez) && ejecuta;
                match(";");
                resultado = Incremento(ejecuta);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(ejecuta, primeraVez);
                }

                if (getValor(variable) < resultado)
                {
                    if (primeraVez)
                    {
                        asm.WriteLine("INC " + variable);
                    }
                }
                else if (getValor(variable) > resultado)
                {
                    if (primeraVez)
                    {
                        asm.WriteLine("DEC " + variable);
                    }
                }

                if (ejecuta)
                {
                    //Modifica(variable, resultado);
                    archivo.DiscardBufferedData();
                    character = inicia - variable.Length - 1;
                    archivo.BaseStream.Seek(character, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;

                    Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                    Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                    if (tipoDatoVariable >= tipoDatoResultado)
                    {
                        Modifica(variable, resultado);
                    }
                    else
                    {
                        throw new Error(" de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                    }
                }

                if (primeraVez)
                {
                    asm.WriteLine("JMP " + etiquetaInicio);
                    asm.WriteLine(etiquetaFin + ":");

                }

                primeraVez = false;
            }
            while (ejecuta);
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool ejecuta)
        {
            float valor = 0;

            if (!Existe(getContenido()))
            {
                throw new Error(getContenido() + "de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            string variable = getContenido();
            match(Tipos.Identificador);

            if (getContenido() == "++")
            {
                match("++");
                valor = getValor(variable) + 1;
            }
            else
            {
                match("--");
                valor = getValor(variable) - 1;
            }
            return valor;

        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion(string etiqueta, bool primeraVez)
        {

            Expresion(primeraVez);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(primeraVez);
            float R1 = stack.Pop();
            float R2 = stack.Pop();

            if (primeraVez)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX,BX");
            }

            switch (operador)
            {
                case "==":
                    if (primeraVez) asm.WriteLine("JNE " + etiqueta);
                    return R2 == R1;
                case ">":
                    if (primeraVez) asm.WriteLine("JBE " + etiqueta);
                    return R2 > R1;
                case ">=":
                    if (primeraVez) asm.WriteLine("JB " + etiqueta);
                    return R2 >= R1;
                case "<":
                    if (primeraVez) asm.WriteLine("JAE " + etiqueta);
                    return R2 < R1;
                case "<=":
                    if (primeraVez) asm.WriteLine("JA " + etiqueta);
                    return R2 <= R1;
                default:
                    if (primeraVez) asm.WriteLine("JE " + etiqueta);
                    return R2 != R1;
            }
        }

        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta, bool primeraVez)
        {
            match("if");
            match("(");

            if (primeraVez)
            {
                asm.WriteLine("; if: " + contadorIf);
            }

            string etiqueta = "Eif" + contadorIf++;

            if (primeraVez)
            {
                contadorIf++;
            }

            string etiquetaelse = "Eelse" + contadorIf++;
            bool evaluacion = Condicion(etiqueta, primeraVez);
            match(")");

            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion && ejecuta, primeraVez);
            }
            else
            {
                Instruccion(evaluacion && ejecuta, primeraVez);
            }

            if (getContenido() == "else")
            {
                match("else");
                if (primeraVez)
                {
                    asm.WriteLine("; else: " + contadorElse);
                    asm.WriteLine("JMP " + etiquetaelse);
                    asm.WriteLine(etiqueta + ":");
                }

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!evaluacion && ejecuta, primeraVez);
                }
                else
                {
                    Instruccion(!evaluacion && ejecuta, primeraVez);
                }

                if (primeraVez)
                {
                    asm.WriteLine(etiquetaelse + ":");
                }

                if (primeraVez)
                {
                    contadorElse++;
                }
            }
        }

        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta, bool primeraVez)
        {
            match("printf");
            match("(");

            if (ejecuta)
            {
                string cadena = getContenido().Trim('"');
                cadena = cadena.Replace("\\n", "\n");
                cadena = cadena.Replace("\\t", "\t");
                Console.Write(cadena);

                if (primeraVez)
                {
                    asm.WriteLine("print '' \n" + "print '" + getContenido().Replace("\"", "").Replace("\\n", "'\nprintn ' ' \nprint '").Replace("\\t", "\t") + "'");
                }
            }
            else
            {
                if (primeraVez)
                {
                    asm.WriteLine("print '' \n" + "print '" + getContenido().Replace("\"", "").Replace("\\n", "'\nprintn ' ' \nprint '").Replace("\\t", "\t") + "'");
                }
            }
            match(Tipos.Cadena);

            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }

                if (ejecuta)
                {
                    if (primeraVez)
                    {
                        asm.WriteLine("MOV AX," + getContenido());
                        asm.WriteLine("CALL print_num");
                        asm.WriteLine("PRINTN ''");
                    }

                    Console.Write(getValor(getContenido()));
                }
                else
                {
                    if (primeraVez)
                    {
                        asm.WriteLine("MOV AX," + getContenido());
                        asm.WriteLine("PRINTN ''");
                        asm.WriteLine("CALL print_num");
                        asm.WriteLine("PRINTN ''");
                    }
                }
                match(Tipos.Identificador);
            }

            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta, bool primeraVez)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");

            if (!Existe(getContenido()))
            {
                throw new Error("de  sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }

            string var = getContenido();
            match(Tipos.Identificador);

            if (ejecuta)
            {
                if (primeraVez)
                {
                    asm.WriteLine("Call scan_num");
                    asm.WriteLine("MOV " + var + ", CX");
                    asm.WriteLine("MOV AX," + var);
                    asm.WriteLine("PRINTN ''");
                }

                string captura = "" + Console.ReadLine();
                float resultado = float.Parse(captura);
                Modifica(var, resultado);
                Variable.TiposDatos tipoDatoVariable = getTipo(var);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                if (tipoDatoVariable >= tipoDatoResultado)
                {
                    Modifica(var, resultado);
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                }

            }

            else
            {
                if (primeraVez)
                {
                    asm.WriteLine("Call scan_num");
                    asm.WriteLine("MOV " + var + ", CX");
                    asm.WriteLine("MOV AX," + var);
                    asm.WriteLine("PRINTN ''");
                }
            }

            match(")");
            match(";");
        }
        //Main -> void main() BloqueInstrucciones
        private void Main(bool ejecuta)
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(ejecuta, true);
        }
        //Expresion -> Termino MasTermino
        private void Expresion(bool primeraVez)
        {
            Termino(primeraVez);
            MasTermino(primeraVez);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(primeraVez);
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();

                if (primeraVez)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }

                if (operador == "+")
                {
                    stack.Push(R1 + R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("ADD   AX, BX");//
                        asm.WriteLine(" PUSH AX");
                    }
                }

                else
                {
                    stack.Push(R1 - R2);
                    if (primeraVez)
                    {
                        asm.WriteLine("SUB  AX, BX");
                        asm.WriteLine("PUSH AX");
                    }
                }
            }
        }

        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(primeraVez);
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();

                if (primeraVez)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }

                if (operador == "*")
                {
                    stack.Push(R1 * R2);

                    if (primeraVez)
                    {
                        asm.WriteLine(" MUL BX");
                        asm.WriteLine(" PUSH AX");
                    }
                }
                else if (operador == "/")
                {
                    stack.Push(R1 / R2);

                    if (primeraVez)
                    {
                        asm.WriteLine(" DIV  BX");
                        asm.WriteLine(" PUSH AX");
                    }
                }
                else
                    stack.Push(R1 % R2);

                if (primeraVez)
                {
                    asm.WriteLine("DIV  BX");
                    asm.WriteLine(" PUSH DX");
                }
            }
        }

        //Termino -> Factor PorFactor
        private void Termino(bool primeraVez)
        {
            Factor(primeraVez);
            PorFactor(primeraVez);
        }

        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());

                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, " + getContenido());//
                    asm.WriteLine("PUSH AX");
                }

                stack.Push(float.Parse(getContenido()));

                if (tipoDatoExpresion < getTipo(float.Parse(getContenido())))
                {
                    tipoDatoExpresion = getTipo(float.Parse(getContenido()));
                }

                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }

                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }

                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);

                if (tipoDatoExpresion < getTipo(getContenido()))
                {
                    tipoDatoExpresion = getTipo(getContenido());
                }

            }
            else
            {
                bool huboCast = false;
                Variable.TiposDatos tipoDatoCast = Variable.TiposDatos.Char;
                match("(");

                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCast = true;
                    switch (getContenido())
                    {
                        case "int": tipoDatoCast = Variable.TiposDatos.Int; break;
                        case "float": tipoDatoCast = Variable.TiposDatos.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }

                Expresion(primeraVez);
                match(")");

                if (huboCast)
                {
                    tipoDatoExpresion = tipoDatoCast;
                    stack.Push(castea(stack.Pop(), tipoDatoCast));
                }
            }
        }

        float castea(float resultado, Variable.TiposDatos tipoDato)
        {

            if (tipoDato == Variable.TiposDatos.Char)
            {
                resultado = (float)Math.Round(resultado);
                asm.WriteLine("MOV AX, " + resultado);
                asm.WriteLine("MOV BX, 256");
                resultado = (char)resultado % 256;
                asm.WriteLine("DIV BX");
                asm.WriteLine("MOV AX,DX ");
                asm.WriteLine("PUSH AX");
                asm.WriteLine("XOR DX , DX");
            }

            else if (tipoDato == Variable.TiposDatos.Int)
            {
                resultado = (float)Math.Round(resultado);
                resultado = (int)resultado % 65556;
            }

            return resultado;
        }
    }
}