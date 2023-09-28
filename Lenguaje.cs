using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

/*
    Requerimiento 1: Implementar la ejecucion del while
    Requerimiento 2: Implementar la ejecucion del do - while
    Requerimiento 3: Implementar la ejecucion del for
    Requerimiento 4: Marcar errores semanticos -> Asignacion
    Requerimiento 5: CAST
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
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
           
        private Variable.TiposDatos getTipo(float resulatdo)
        {
            if (resulatdo % 1 !=0 )
            {
                return Variable.TiposDatos.Float;
            }
            else if (resulatdo < 256)
            {
                return Variable.TiposDatos.Char;
            }
                       
            else if (resulatdo < 65536)
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
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            match("}");
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

            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                float val = getValor(variable);
                if (getContenido() == "++")
                {
                    match("++");
                    //Agrego
                    val++;
                    Modifica(variable, val);
                    stack.Push(val);
                }
                else
                {
                    match("--");
                    //Agrego
                    val--;
                    Modifica(variable, val);
                    stack.Push(val);
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {

                if (getContenido() == "+=")
                {
                    match("+=");
                    //Agrego
                    Expresion();
                    float val = getValor(variable);
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (!Existe(getContenido()))
                        {
                            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                        }
                        string variable2 = getContenido();
                        match(Tipos.Identificador);
                        float val2 = getValor(variable2);
                        val += val2;
                    }
                    else
                    {
                        val += stack.Pop();
                    }
                    Modifica(variable, val);
                    stack.Push(val);
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    //Agrego
                    Expresion();
                    float val = getValor(variable);
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (!Existe(getContenido()))
                        {
                            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                        }
                        string variable2 = getContenido();
                        match(Tipos.Identificador);
                        float val2 = getValor(variable2);
                        val -= val2;
                    }
                    else
                    {
                        val -= stack.Pop();
                    }
                    Modifica(variable, val);
                    stack.Push(val);
                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    //Agrego
                    Expresion();
                    float val = getValor(variable);
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (!Existe(getContenido()))
                        {
                            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                        }
                        string variable2 = getContenido();
                        match(Tipos.Identificador);
                        float val2 = getValor(variable2);
                        val *= val2;
                    }
                    else
                    {
                        val *= stack.Pop();
                    }
                    Modifica(variable, val);
                    stack.Push(val);
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    //Agrego
                    Expresion();
                    float val = getValor(variable);
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (!Existe(getContenido()))
                        {
                            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                        }
                        string variable2 = getContenido();
                        match(Tipos.Identificador);
                        float val2 = getValor(variable2);
                        val /= val2;
                    }
                    else
                    {
                        val /= stack.Pop();
                    }
                    Modifica(variable, val);
                    stack.Push(val);
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    //Agrego
                    Expresion();
                    float val = getValor(variable);
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (!Existe(getContenido()))
                        {
                            throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                        }
                        string variable2 = getContenido();
                        match(Tipos.Identificador);
                        float val2 = getValor(variable2);
                        val %= val2;
                    }
                    else
                    {
                        val %= stack.Pop();
                    }
                    Modifica(variable, val);
                    stack.Push(val);
                }
                //Expresion();
            }
            float resultado = stack.Pop();
            log.WriteLine(" = " + resultado);
            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo (variable);
                Variable.TiposDatos tipoDatoResultado = getTipo (resultado);

                //Console.WriteLine(variable + " = " + tipoDatoVariable);
                //Console.WriteLine(resultado + " = " + tipoDatoResultado);
                Modifica(variable, resultado);
            }
            match(";");
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }

        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta)
        {
            match("do");
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
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion(ejecuta);
            Condicion();
            match(";");
            Incremento(ejecuta);
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
        }
        //Incremento -> Identificador ++ | --
        private void Incremento(bool ejecuta)
        {
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
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
            bool condicional = Condicion();
            bool evaluacion = condicional && ejecuta;
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
                    BloqueInstrucciones(!condicional && ejecuta);
                }
                else
                {
                    Instruccion(!condicional && ejecuta);
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
                String imprime = getContenido();
                imprime = imprime.Replace("\\t", "\t");
                imprime = imprime.Replace("\\n", "\n");
                imprime = imprime.Replace('"', '\0');

                Console.Write(imprime);
            }
            match(Tipos.Cadena);
            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                string variable = getContenido();
                match(Tipos.Identificador);
                string imprime = "" + getValor(variable);
                if (ejecuta)
                {
                    Console.Write(imprime);
                }
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
                //Agrego
                if (captura.All(char.IsDigit))
                {
                    float resultado = float.Parse(captura);
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Error("de sintaxis, la cadena introducida SOLO DEBE CONTENER DIGITOS", log, linea, columna);
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
            Console.WriteLine(getContenido() + " " + float.Parse(getContenido()));
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}