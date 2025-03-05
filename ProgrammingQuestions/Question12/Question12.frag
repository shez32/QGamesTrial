#ifdef GL_ES
precision mediump float;
#endif

float f(float x, float dx0, float dx1) {
    if (x < 0.0) return 0.0;
    if (x > 1.0) return 1.0;
    
    // Cubic Hermite spline
    float x2 = x * x;
    float x3 = x2 * x;
    
    float h00 = 2.0 * x3 - 3.0 * x2 + 1.0;
    float h10 = x3 - 2.0 * x2 + x;
    float h01 = -2.0 * x3 + 3.0 * x2;
    float h11 = x3 - x2;
    
    return h00 * 0.0 + h10 * dx0 + h01 * 1.0 + h11 * dx1;
}

void main() {
    float x = gl_FragCoord.x / 500.0; // Normalize x to [0, 1] based on screen width
    float dx0 = 2.0; // Slope at x = 0
    float dx1 = -1.0; // Slope at x = 1
    
    float y = f(x, dx0, dx1);
    
    gl_FragColor = vec4(y, y, y, 1.0); // Visualize the function as grayscale
}