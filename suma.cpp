#include<stdio.h>
#include<math.h>
#include<iostream>

float a,b,c;

void main() // Funcion principal
{
    a=(3+5)*8-(10-4)/2;
    b=8;
    printf("Valor de c = ",c);
    scanf("%f",&c);
    printf("Valor de b = ",b);
    if (c%2==0)
    {
        printf("\nc es par\t\tITQ");
        if (c==10)
            printf("El valor de c = ",c);
    }
    else
    {
        printf("\nc es impar\t\tITQ");
    }
    printf("Valor de b = ",b);
    b++;
    c--;
    c+=(10-b);
    b-=9;
    printf("El valor de a = ",a);
    printf("El valor de b = ",b);
    printf("El valor de d = ",d);
}
