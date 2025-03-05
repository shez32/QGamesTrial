#ifdef GL_ES
precision mediump float;
#endif

float mirror_repeat(float x, float a, float b) {
    float interval = b - a;
    x = x - a; // Shift x to start from 0
    x = mod(x, 2.0 * interval); // Wrap x into [0, 2*interval]
    x = abs(x - interval); // Mirror within [0, interval]
    x = x + a; // Shift back to [a, b]
    return x;
}

void main() {
    float x = gl_FragCoord.x / 500.0 * 4.0; // Scale x to [0, 4] for visualization
    float a = 1.0; // Start of the interval
    float b = 2.0; // End of the interval
    
    float y = mirror_repeat(x, a, b);
    
    gl_FragColor = vec4(y, y, y, 1.0); // Visualize the function as grayscale
}