using UnityEngine;

[RequireComponent(typeof(Light), typeof(SpriteRenderer))]
public class Luz : Interactuable {
    [Header("Configuración de la luz")]
    [Tooltip("Define la intensidad inicial de la luz")]
    [SerializeField] private float intensidadInicial = 0.5f;
    [Tooltip("Define el rango inicial de la luz")]
    [SerializeField] private float rangoInicial = 5.0f;
    [Tooltip("Establece las variantes de color a utilizar")]
    [SerializeField] private Color[] colores = {Color.white,};
    [Space(5)]
    [Tooltip("Permite variaciones de la luz")]
    [SerializeField] private bool esEstatica = true;
    [Space(5)]
    [Tooltip("Define la velocidad de cambio en la intensidad de la luz (avanzado)")]
    [SerializeField, Min(0)] private float velocidadIntensidad = 0.75f;
    [Tooltip("Define la velocidad de cambio en el rango de la luz (avanzado)")]
    [SerializeField, Min(0)] private float velocidadRango = 1.0f;
    [Tooltip("Define la velocidad de cambio en el rango de la luz (avanzado)")]
    [SerializeField, Min(0)] private float velocidadColor = 0.5f;
    [Space(5)]
    [Tooltip("Establece una variación máxima de la intensidad (-|+, avanzado)")]
    [SerializeField, Min(0)] private float variacion = 0.1f;
    [Tooltip("Establece una variación máxima del rango (-|+, avanzado)")]
    [SerializeField, Min(0)] private float rango = 1.0f;
    [Space(5)]
    [Tooltip("Controla si el cambio de rango es intermitente o fluido")]
    [SerializeField] private bool rangoSimple = false;
    [Tooltip("Controla si el cambio de intensidad es intermitente o fluido")]
    [SerializeField] private bool intensidadSimple = false;
    [Tooltip("Controla si el cambio de color es intermitente o fluido")]
    [SerializeField] private bool colorSimple = false;

    [Space(20)]
    [Header("DEV (Variables de control)")]
    [Tooltip("Arrastra un sprite a mostrar default")]
    [SerializeField] private Sprite spriteOff;
    [Tooltip("Arrastra un sprite a mostrar al activarse")]
    [SerializeField] private Sprite spriteOn;
    private Light luzComp;
    private float intensidadFinal;
    private float rangoFinal;
    private Color colorFinal;
    private int colorActualIndex = 0;
    private float tiempoIntensidad = 0.0f;
    private float tiempoRango = 0.0f;
    private float tiempoColor = 0.0f;

    protected void Awake() {
        //Inicializar el componente de luz
        luzComp = GetComponent<Light>();
    }

    protected void Start() {
        //Establecemos los valores iniciales
        intensidadFinal = intensidadInicial;
        rangoFinal = rangoInicial;
        luzComp.intensity = intensidadInicial;
        luzComp.range = rangoInicial;
        colorFinal = colores[colorActualIndex];
        luzComp.color = colorFinal;
    }

    protected override void Update() {
        //Llamar a la funcionalidad heredada
        base.Update();

        //Deshabilitamos o procesamos
        if (activo) {
            GetComponent<SpriteRenderer>().sprite = spriteOn;
            if (esEstatica) ProcesarCambios();
            else { 
                luzComp.intensity = intensidadInicial;
                luzComp.range = rangoInicial;
                luzComp.color = colores[(colorActualIndex + 1) % colores.Length];
            }
        } else {
            GetComponent<SpriteRenderer>().sprite = spriteOff;
            luzComp.intensity = 0;
            luzComp.range = 0;
            luzComp.color = Color.black;
        }
    }

    private void ProcesarCambios() {
        //Manejar cambios de intensidad
        if (velocidadIntensidad != 0) {
            //Inmediato
            if (intensidadSimple) {
                tiempoIntensidad += Time.deltaTime * velocidadIntensidad;
                if (tiempoIntensidad >= velocidadIntensidad) {
                    intensidadFinal = (luzComp.intensity == intensidadInicial) ? intensidadInicial - variacion : intensidadInicial;
                    luzComp.intensity = intensidadFinal;
                    tiempoIntensidad = 0.0f;
                }
            //Suave
            } else {
                luzComp.intensity = Mathf.Lerp(luzComp.intensity, intensidadFinal, Time.deltaTime * velocidadIntensidad);
                if (Mathf.Abs(luzComp.intensity - intensidadFinal) < 0.01f) {
                    intensidadFinal = intensidadInicial + Random.Range(-variacion, variacion);
                }
            }
        }

        //Manejar cambios de rango
        if (velocidadRango != 0) {
            //Inmediato
            if (rangoSimple) {
                tiempoRango += Time.deltaTime * velocidadRango;
                if (tiempoRango >= velocidadRango) {
                    rangoFinal = (luzComp.range == rangoInicial) ? rangoInicial - rango : rangoInicial;
                    luzComp.range = rangoFinal;
                    tiempoRango = 0.0f;
                }
            //Suave
            }
            else {
                luzComp.range = Mathf.Lerp(luzComp.range, rangoFinal, Time.deltaTime * velocidadRango);
                if (Mathf.Abs(luzComp.range - rangoFinal) < 0.1f) {
                    rangoFinal = rangoInicial + Random.Range(-rango, rango);
                }
            }
        }

        //Manejar cambios de color
        if (velocidadColor != 0) {
            //Inmediato
            if (colorSimple) {
                tiempoColor += Time.deltaTime * velocidadColor;
                if (tiempoColor >= velocidadColor) {
                    colorActualIndex = (colorActualIndex + 1) % colores.Length;
                    colorFinal = colores[colorActualIndex];
                    luzComp.color = colorFinal;
                    tiempoColor = 0.0f;
                }
            //Suave
            } else {
                luzComp.color = Color.Lerp(luzComp.color, colorFinal, Time.deltaTime * velocidadColor);
                if (Mathf.Abs(luzComp.color.r - colorFinal.r) < 0.05f &&
                    Mathf.Abs(luzComp.color.g - colorFinal.g) < 0.05f &&
                    Mathf.Abs(luzComp.color.b - colorFinal.b) < 0.05f) {
                    colorActualIndex = (colorActualIndex + 1) % colores.Length;
                    colorFinal = colores[colorActualIndex];
                }
            }
        }
    }
}