; Autor: Shaina Alexandra Xochitiotzi Rojas
; 11/13/2023 12:44:05 AM
include 'emu8086.inc'
org 100h
MOV AX, 258
PUSH AX
POP AX
;Asignacion a
MOV a, AX
MOV AX, a
PUSH AX
MOV AX, 258
MOV BX, 256
DIV BX
MOV AX,DX 
PUSH AX
XOR DX , DX
POP AX
;Asignacion a
MOV a, AX
MOV AX, 8
PUSH AX
POP AX
ADD  a ,AX
MOV AX, 10
PUSH AX
POP BX
MOV BX,a
MUL BX
MOV a ,AX
MOV AX, 100
PUSH AX
POP BX
MOV BX,a
DIV BX
MOV a ,AX
print '' 
print 'Valor Casteado de a: '
MOV AX,a
CALL print_num
PRINTN ''
print '' 
print ''
printn ' ' 
print 'Digite el valor de altura: '
Call scan_num
MOV altura, CX
MOV AX,altura
PRINTN ''
print '' 
print ''
printn ' ' 
print 'for:'
printn ' ' 
print ''
; For: 1
MOV AX, 1
PUSH AX
POP AX
;Asignacion i
MOV i, AX
InicioFor1:
MOV AX, i
PUSH AX
MOV AX, altura
PUSH AX
POP BX
POP AX
CMP AX,BX
JA FinFor1
print '' 
print '	'
; For: 2
MOV AX, 250
PUSH AX
POP AX
;Asignacion j
MOV j, AX
InicioFor2:
MOV AX, j
PUSH AX
MOV AX, 250
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
ADD   AX, BX
 PUSH AX
POP BX
POP AX
CMP AX,BX
JAE FinFor2
; if: 1
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV  BX
 PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX,BX
JNE Eif1
print '' 
print '-'
; else: 1
JMP Eelse3
Eif1:
print '' 
print '+'
Eelse3:
INC j
JMP InicioFor2
FinFor2:
print '' 
print ''
printn ' ' 
print ''
INC i
JMP InicioFor1
FinFor1:
; For: 3
; For: 4
; For: 5
; For: 6
; For: 7
; For: 8
; For: 9
; For: 10
; For: 11
; For: 12
; For: 13
; For: 14
print '' 
print ''
printn ' ' 
print 'while:'
printn ' ' 
print ''
MOV AX, 1
PUSH AX
POP AX
;Asignacion i
MOV i, AX
; While: 1
InicioWhile1:
MOV AX, i
PUSH AX
MOV AX, altura
PUSH AX
POP BX
POP AX
CMP AX,BX
JA FinWhile1
print '' 
print '	'
MOV AX, 250
PUSH AX
POP AX
;Asignacion j
MOV j, AX
; While: 2
InicioWhile2:
MOV AX, j
PUSH AX
MOV AX, 250
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
ADD   AX, BX
 PUSH AX
POP BX
POP AX
CMP AX,BX
JAE FinWhile2
; if: 184
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV  BX
 PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX,BX
JNE Eif184
print '' 
print '-'
; else: 2
JMP Eelse186
Eif184:
print '' 
print '+'
Eelse186:
INC  j
JMP InicioWhile2
FinWhile2:
INC  i
print '' 
print ''
printn ' ' 
print ''
JMP InicioWhile1
FinWhile1:
; While: 3
; While: 4
; While: 5
; While: 6
; While: 7
; While: 8
; While: 9
; While: 10
; While: 11
; While: 12
; While: 13
; While: 14
print '' 
print ''
printn ' ' 
print 'do:'
printn ' ' 
print ''
MOV AX, 1
PUSH AX
POP AX
;Asignacion i
MOV i, AX
InicioDo1:
print '' 
print '	'
MOV AX, 250
PUSH AX
POP AX
;Asignacion j
MOV j, AX
InicioDo2:
; if: 367
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV  BX
 PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX,BX
JNE Eif367
print '' 
print '-'
; else: 3
JMP Eelse369
Eif367:
print '' 
print '+'
Eelse369:
INC  j
MOV AX, j
PUSH AX
MOV AX, 250
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
ADD   AX, BX
 PUSH AX
POP BX
POP AX
CMP AX,BX
JAE FinDo2
JMP InicioDo2
FinDo2:
INC  i
print '' 
print ''
printn ' ' 
print ''
MOV AX, i
PUSH AX
MOV AX, altura
PUSH AX
POP BX
POP AX
CMP AX,BX
JA FinDo1
JMP InicioDo1
FinDo1:
int 20h
RET
define_scan_num
define_print_num
define_print_num_uns
; V a r i a b l e s
altura dw 0h
i dw 0h
j dw 0h
k dw 0h
a dw 0h
b dw 0h
END
