# Base para juego platformer 2D



Este proyecto es una base para practicar el diseño de niveles de juegos 2D.



Incluye diferentes elementos que se pueden utilizar para crear espacios interesantes.



Desarrollado en la versión 2022.3 de Unity.





## Prefabs incluidos



El proyecto incluye los siguientes prefabs:





### Objetos interactuables



Objetos no activos en la escena que afectan el entorno o al jugador de alguna forma



#### Antorchas



Luces que se pueden poner en el nivel y que se pueden activar o desactivar con otros elementos.



Hay una antorcha simple y una antorcha compleja. La diferencia entre ellas son las

opciones de desarrollador adicionales.



#### Banderas



Objetos que pueden activar a otros objetos o cambiar la escena.



El script **Bandera** tiene las siguientes opciones:



* Activo (booleano): Define si el objeto funciona.
* Marcada (booleano): Cambia de estado cuando el jugador ya la ha usado.
* Acciones (lista de gameObjects): Los objetos interactuables que habilita.
* Cambiar escenas (booleano): Define si la bandera cambiará escenas.
* Escena (String): La escena a cambiar si la opción anterior está activa.
* Nivel
* Valor



#### Botones



Hay 4 variantes. **Simples** y **avanzados** que además pueden ser **directos** o **resistentes**.

La diferencia entre los botones simples y avanzados son las opciones adicionales de desarrollador.

La diferencia entre los botones directos y resistentes es el valor de la opción de rebote.



El script **Boton** tiene las siguientes opciones:



* Activo (booleano): Define si el objeto funciona.
* Acciones (lista de gameObjects): Los objetos interactuables que habilita.
* Valor (booleano): El estado actual del botón.
* Mantener (booleano): Si no se activa el botón se desactiva después de cierto tiempo aunque el jugador esté sobre él.
* Duración (float): La duración en segundos para pulsaciones alternantes.
* Resistir (booleano): Medir la fuerza que imprime el jugador en el botón para activarlo acordemente.
* Fuerza (float): La fuerza necesari apara activar el botón.
* Pesar (booleano): Usa el peso definido para activar el botón.
* Peso (float): El peso a usar para activar el botón.
* Rebote (booleano): Si se activa arroja al jugador lejos luego de que se desactiva el botón.
* Impulso (float): La fuerza con la que se aleja al jugador.







#### Enlaces



Conectan diferentes partes del nivel



#### Objetos



Recolectables que se pueden poner en el nivel.



#### Obstáculos



Se dividen en 3 categorías:



##### Bloques



Se pueden utilizar para bloquear partes del escenario hasta que se active un objeto interactuable.



##### Peligros



Hacen daño al jugador.



##### Plataformas



Pueden conectar partes del escenario separadas por espacios vacíos.



#### Palancas



Hay 4 variantes. **Simples** y **avanzadas** que además pueden ser **alternantes** o **repetitivas**.

La diferencia entre las palancas simples y avanzados son las opciones adicionales de desarrollador.

La diferencia entre los botones directos y resistentes es el valor de la opción de rebote.





#### Propulsores



Aplican fuerza al jugador. Tienen los siguientes atributos:



* Afectables: Las capas de interacción que afecta.
* Fuerza (float): La magnitud de la fuerza que se aplica al objeto que entra al campo.
* Invertir (booleano): Define si la fuerza es repulsiva o propulsiva.
* Ángulo dinámico (booleano): Permite personalizar el ángulo en el que se aplica la fuerza.
* Ángulo de fuerza (float): EL ángulo de la fuerza que se aplica.
* Incremental (booleano): Multiplicador por cercanía.



Hay 4 tipos, que se diferencian por los valores que tienen estos atributos:



* Gravedad cero: Nulifica el efecto de la fuerza de gravedad del proyecto.
* Impulsor: 
* Magneto:
* Repulsor:



#### Otros



Objetos que no pertenecen a los grupos anteriores.



##### Resorte



Lanza al jugador con una fuerza predeterminada.



##### Puerta



Cambia la escena cuando la toca el jugador.



##### Portal



Cambia la posición del jugador al portal al que está conectado.



##### Generador



Crea copias de prefabs.





### Entidades



Objetos activos en la escena



#### Jugador



Puede moverse, saltar, deslizarse en los muros.





#### Abeja



Bzzz



#### Rana



Croak

