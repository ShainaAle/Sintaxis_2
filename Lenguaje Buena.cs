using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        Variable.TiposDatos tipoDatoExpresion;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            tipoDatoExpresion = Variable.TiposDatos.Char;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main(true);
            Imprime();
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDatos() + " = " + v.getValor());
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
            if (resultado < 256)
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
        private float BloqueInstrucciones(bool ejecuta)
        {
            float valor = 0;
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
                String var = getContenido();
                valor = getValor(var) + 1;
            }
            match("}");
            return valor;
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta)
        {
            float resultado = 0;
            tipoDatoExpresion = Variable.TiposDatos.Char;
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            log.Write(getContenido() + " = ");
            string variable = getContenido();
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                Expresion();
                resultado = stack.Pop();
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    resultado = getValor(variable) + 1;
                }
                else
                {
                    match("--");
                    resultado = getValor(variable) - 1;
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {
                resultado = getValor(variable);
                if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion();
                    resultado += stack.Pop();
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion();
                    resultado -= stack.Pop();
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion();
                    resultado *= stack.Pop();
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion();
                    resultado /= stack.Pop();
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    Expresion();
                    resultado %= stack.Pop();
                }
            }
            log.WriteLine(" = " + resultado);
            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                if (tipoDatoVariable >= tipoDatoExpresion)
                {
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar un <" + tipoDatoExpresion + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                }
            }
            match(";");
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            int principio = caracter;
            int lineaInicio = linea;

            String var;
            var = getContenido();

            do
            {
                match("while");
                match("(");
                ejecuta = Condicion() && ejecuta;
                match(")");

                if (getContenido() == "{")
                    BloqueInstrucciones(ejecuta);
                else
                    Instruccion(ejecuta);

                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = principio - var.Length - 1;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                    caracter = principio;
                }

            } while (ejecuta);
        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta)
        {
            int principio = caracter;
            int lineaInicio = linea;

            match("do");
            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                match("while");
                match("(");
                ejecuta = Condicion() && ejecuta;
                match(")");
                match(";");

                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = principio;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                }
            } while (ejecuta);
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion(ejecuta);

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();

            log.WriteLine("For: " + variable);

            do
            {
                ejecuta = Condicion() && ejecuta;
                match(";");
                resultado = Incremento(ejecuta);

                Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);

                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                if (ejecuta)
                {

                    if (tipoDatoResultado <= tipoDatoExpresion)
                    {
                        Modifica(variable, resultado);
                        archivo.DiscardBufferedData();
                        caracter = inicia - variable.Length - 1;
                        archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                        nextToken();
                        linea = lineaInicio;
                    }
                    else
                    {
                        throw new Error("de semantica, no se puede asignar un <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                    }

                }
            } while (ejecuta);

        }
        //Incremento -> Identificador ++ | --
        private float Incremento(bool ejecuta)
        {
            float valor = 0;
            String var = getContenido();
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                valor = getValor(var) + 1;
            }
            else
            {
                match("--");
                valor = getValor(var) - 1;
            }
            return valor;
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float R1 = stack.Pop();
            float R2 = stack.Pop();

            switch (operador)
            {
                case "==": return R2 == R1;
                case ">": return R2 > R1;
                case ">=": return R2 >= R1;
                case "<": return R2 < R1;
                case "<=": return R2 <= R1;
                default: return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta)
        {
            match("if");
            match("(");
            bool condicionalElse = Condicion();
            bool evaluacion = condicionalElse && ejecuta;
            //Console.WriteLine(evaluacion);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            if (getContenido() == "else")
            {
                match("else");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!condicionalElse && ejecuta);
                }
                else
                {
                    Instruccion(!condicionalElse && ejecuta);
                }
            }

        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta)
        {
            match("printf");
            match("(");
            if (ejecuta)
            {
                string cadena = getContenido().TrimStart('"');
                cadena = cadena.Remove(cadena.Length - 1);
                cadena = cadena.Replace(@"\n", "\n");
                cadena = cadena.Replace("\\n", "\n");
                cadena = cadena.Replace('"', '\0');
                Console.Write(cadena);
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
                    Console.Write(getValor(getContenido()));

                match(Tipos.Identificador);

            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            if (ejecuta)
            {
                string captura = "" + Console.ReadLine();
                if (captura.All(char.IsDigit))
                {
                    float resultado = float.Parse(captura);
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Error("de sintáxis, se espera un valor numérico", log, linea, columna);
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
            BloqueInstrucciones(ejecuta);
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (operador == "+")
                    stack.Push(R1 + R2);
                else
                    stack.Push(R1 - R2);
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (operador == "*")
                    stack.Push(R1 * R2);
                else if (operador == "%")
                    stack.Push(R1 % R2);
                else
                    stack.Push(R1 / R2);
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido()));
                if (tipoDatoExpresion < getTipo(float.Parse(getContenido())))
                {
                    tipoDatoExpresion = getTipo(float.Parse(getContenido()));
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                bool huboCast = true;
                Variable.TiposDatos tipoDatoCast = Variable.TiposDatos.Char;

                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
                if (tipoDatoExpresion < getTipo(getContenido()))
                {
                    tipoDatoExpresion = getTipo(getContenido());
                    huboCast = false;
                }
                else if (huboCast)
                {
                    tipoDatoExpresion = tipoDatoCast;
                    stack.Push(castea(stack.Pop(), tipoDatoCast));
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
                Expresion();
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
                if (resultado % 1 > 0)
                {
                    resultado = (float)Math.Round(resultado);
                }
                resultado = resultado % 256;
            }
            else if (tipoDato == Variable.TiposDatos.Int)
            {
                if (resultado % 1 > 0)
                    resultado = (float)Math.Round(resultado);

                resultado = resultado % 65536;
            }
            return resultado;
        }
    }
}