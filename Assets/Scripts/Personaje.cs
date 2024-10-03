using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Personaje : MonoBehaviour {
    //Variables de personalización
    [Header("Configuración del jugador")]
    [Tooltip("Asigna tiempo en aire permitido para saltar")]
    [SerializeField] private float coyoteTime = 0.1f;
    [Tooltip("Permite movimiento al estar en el aire")]
    [SerializeField] private bool movimientoAereo = false;
    [Tooltip("Habilita si puede realizar saltos en la pared")]
    [SerializeField] private bool saltoEnPared = false;
    [Tooltip("Limita de vida base del personaje")]
    [SerializeField] public int vidaBase = 6;
    [Tooltip("Establece vida actual del personaje")]
    [SerializeField] public int vida = 6;
    [Tooltip("Asigna una fuerza para movimiento horizontal")]
    [SerializeField] private float fuerzaMovimiento = 0.9f;
    [Tooltip("Asigna una velocidad máxima para movimiento")]
    [SerializeField] private float velocidadMaxima = 15.0f;
    [Tooltip("Asigna una fuerza para saltar")]
    [SerializeField] private float fuerzaSalto = 10.0f;
    [Tooltip("Establece un multiplicador al movimiento sobre superficies")]
    [SerializeField] private float resistencia = 0.85f;
    [Tooltip("Establece capas de superficies que permiten la interacción para caminar")]
    [SerializeField] private LayerMask pisables;
    [Tooltip("Asigna un rango de detección de superficies para caminado")]
    [SerializeField] private float largoRaycast = 0.55f;
    [Space(20)]

    //Variables de funcionamiento
    [Header("DEV (Variables de control)")]
    [SerializeField] private GameManager nivel;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator ani;
    [SerializeField] private RectTransform vidas; // PENDIENTE
    [SerializeField] private RectTransform fondos; // PENDIENTE
    [SerializeField] private GameObject prefabBala; // PENDIENTE
    [SerializeField] private float velocidadBala = 20.0f; // PENDIENTE
    [SerializeField] private float beelX = 0.0f;
    [SerializeField] private float inputX = 0.0f;
    [SerializeField] private float airTime = 0.0f;
    [SerializeField] private bool parado = false;
    [SerializeField] private bool cambioEscena = false;

    private void Start() {
        //Definimos el controlador de juego actual
        if (!nivel) nivel = FindAnyObjectByType<GameManager>();
    }

    private void Update() {
        //Detección de piso
        parado = DetectarPlataforma();

        //Deteccion de pulsación horizontal del usuario
        this.inputX = Input.GetAxis("Horizontal");
        //Animar según velocidad horizontal
        this.beelX = Mathf.Sqrt(Mathf.Pow(this.rb.velocity.x, 2));
        this.ani.SetFloat("influenciaMovimiento", this.beelX);
        this.ani.speed = (this.rb.velocity.magnitude / this.velocidadMaxima * 0.5f) + 0.5f;
        
        //Cambiar dirección
        if (this.rb.velocity.x < -0.25f){
            this.sr.flipX = true;
        } else if (this.rb.velocity.x > 0.25f) {
            this.sr.flipX = false;
        }

        //Disparar  PENDIENTE
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Posicionar disparo
            Vector3 posicionDisparo = new Vector3(this.transform.localPosition.x + (this.sr.flipX ? -1 : 1), this.transform.localPosition.y - 0.3f, this.transform.localPosition.z);
            //Crear disparo
            GameObject proyectil = Instantiate(this.prefabBala, posicionDisparo, this.transform.rotation);
            /*if (this.sr.flipX) {
                posicionBala.x = posicionBala.x - 1.0f;
                proyectil.GetComponent<Rigidbody2D>().velocity = new Vector2(this.rb.velocity.x - this.velocidadBala, 0.0f);
            } else {
                posicionBala.x = posicionBala.x + 1.0f;
            }*/
            //posicionDisparo.x = posicionDisparo.x - ((this.rb.velocity.x + 1) / (1 + Mathf.Sqrt(this.rb.velocity.x * this.rb.velocity.x)));
            //Dirigir y lanzar disparo
            proyectil.GetComponent<SpriteRenderer>().flipX = this.sr.flipX;
            proyectil.GetComponent<Rigidbody2D>().velocity = new Vector2(this.velocidadBala * (this.sr.flipX ? -1 : 1), 0.0f);
        }

        //Debug.DrawRay(this.transform.position, this.rb.velocity, Color.red); //?

        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Debug.Log(hit1.collider.gameObject.name); //?
        };
    }

    private void LateUpdate() {
        //Comprobamos vida después
        ComprobarVida();
    }

    private void FixedUpdate() {
        this.airTime = parado ? 0.0f : this.airTime + Time.deltaTime;
        this.ani.SetBool("saltando", this.airTime >= this.coyoteTime);

        //Salto
        if (Input.GetAxis("Vertical") > 0.05f && (!this.ani.GetBool("saltando") || (this.rb.velocity.y < -0.05f && saltoEnPared && (DetectarPared(-1) || DetectarPared(1))))) {
            if (saltoEnPared && (DetectarPared(-1) || DetectarPared(1)) && this.rb.velocity.y < -0.05f) this.rb.velocity += Vector2.right * (this.sr.flipX? 5 : -5);
            this.rb.velocity = new Vector2(this.rb.velocity.x, this.fuerzaSalto);
            //this.rb.AddForce(salto, ForceMode2D.Impulse);
        }

        //Movimiento lateral
        if (Mathf.Sqrt(Mathf.Pow(this.rb.velocity.x, 2)) < this.velocidadMaxima) {
            //Calcular velocidades
            float movimientoHorizontal = (parado || movimientoAereo) ? this.inputX * this.fuerzaMovimiento : this.inputX * this.fuerzaMovimiento / 10.0f;
            Vector2 movTotal;
            //Velocidad si toca el piso
            if (parado) {
                movTotal = new Vector2(movimientoHorizontal + (this.rb.velocity.x * this.resistencia), this.rb.velocity.y);
            //Velocidad estando en el aire
            } else {
                movimientoHorizontal *= 0.3f;
                movTotal = new Vector2(movimientoHorizontal + this.rb.velocity.x, this.rb.velocity.y);
            }
            //Asignar nueva velocidad
            this.rb.velocity = movTotal;
            //this.rb.AddForce(movimiento);

            //Deslizamiento por pared
            if (!parado && this.rb.velocity.y < 0.0f) {
                if ((this.rb.velocity.x < -0.01f && DetectarPared(-1)) || (this.rb.velocity.x > 0.01f && DetectarPared(1))) {
                    this.rb.velocity = new Vector2(this.rb.velocity.x, this.rb.velocity.y / 2.0f);
                }
            }
        }

        float newX = Mathf.Clamp(this.rb.velocity.x, -this.velocidadMaxima, this.velocidadMaxima);
        float newY = Mathf.Clamp(this.rb.velocity.y, -this.velocidadMaxima * 5, this.velocidadMaxima * 5);
        this.rb.velocity = new Vector2(newX, newY);
    }

    private bool DetectarPared(int direccion) {
        //Deteccion con raycast de paredes cercanas
        //Physics2D.queriesHitTriggers = false;
        Vector2 origenRaycast = new Vector2(this.transform.position.x + (direccion * 0.16f), this.transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origenRaycast + (Vector2.down / 2), Vector2.right * direccion, 0.1f, pisables);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(origenRaycast + (Vector2.down / 2), 0.1f * direccion * Vector2.right, Color.red);
        return hit.collider && !hit.collider.usedByEffector;
    }

    private bool DetectarPlataforma() {
        Vector2 offsetRayo = Vector2.left / 5;
        Vector2 posRayo1 = (Vector2)transform.position - offsetRayo + (Vector2.down / 2);
        Vector2 posRayo2 = (Vector2)transform.position + offsetRayo + (Vector2.down / 2);

        bool tocaIzq = DetectarIzq(posRayo1);
        bool tocaDer = DetectarDer(posRayo2);

        return tocaIzq || tocaDer;
    }

    private bool DetectarIzq(Vector3 posRayo) {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(posRayo, Vector2.down, this.largoRaycast, pisables);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(posRayo, Vector2.down * this.largoRaycast, Color.blue);
        return hit.collider != null && rb.velocity.y <= 0.1f;
    }

    private bool DetectarDer(Vector3 posRayo) {
        //Physics2D.queriesHitTriggers = false; //?
        RaycastHit2D hit = Physics2D.Raycast(posRayo, Vector2.down, this.largoRaycast, pisables);
        //Physics2D.queriesHitTriggers = true; //?
        Debug.DrawRay(posRayo, Vector2.down * this.largoRaycast, Color.blue); //?
        return hit.collider != null && rb.velocity.y <= 0.1f;
    }

    private void ComprobarVida() {
        //Actualizar UI
        this.vidas.sizeDelta = new Vector2(50.0f * this.vida, 50.0f);
        this.fondos.sizeDelta = new Vector2(50.0f * this.vidaBase, 50.0f);
    }

    public void Herir(int valor) {
        //Disminuir vida
        if (this.vida >= 1) {
            //Herimos al jugador
            this.vida -= valor;
        }
        if (vida <= 0) { 
            //Reiniciamos desde el ultimo punto de control
            vida = vidaBase;
            if (nivel) transform.position = nivel.SpawnPoint();
            if (cambioEscena) this.GetComponent<ControlEscenas>().CambiarEscena();
            rb.Sleep();
        }
    }

    public void Mover(Vector2 direccion, float fuerza) {
        //Mover con base en direccion
        direccion *= fuerza;
        this.rb.AddForce(direccion, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D colisionado) {
        //Detectar daños TEMPORAL PARA ENEMIGOS
        if (colisionado.gameObject.CompareTag("Enemigo")) {
            this.Herir(1);
        }
    }
}