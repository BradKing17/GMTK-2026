shader_type canvas_item;
//背景填充颜色
uniform vec3 backFillColour : source_color = vec3(0.62,1.00,1.00);
//前景填充靠里的颜色
uniform vec3 frontFillInnerColour : source_color = vec3(0.35,1.00,1.00);
//前景填充靠边的颜色
uniform vec3 frontFillOuterColour : source_color = vec3(0.00,0.35,1.00);
//环的颜色
uniform vec3 ringColour : source_color = vec3(0.00,0.16,0.13);
//玻璃罩阴影颜色
uniform vec3 fresnelColour : source_color = vec3(0.00,0.88,1.00);
//内环发光颜色
uniform vec3 innerRingGlowColour : source_color = vec3(0.00,1.00,1.00);
//填充颜色调整
uniform vec4 fillcolour : source_color = vec4(1);
//环的宽度
uniform float ringWidth = 0.15;
//玻璃球与环的间隔
uniform float innerCircleRadiusOffset = 0.0;
//fill_value,填充球体的值
uniform float fill_value : hint_range(-0, 1) = 0.25;



// 定义函数：计算点到圆弧的距离
float sdArc(in vec2 p, in vec2 sc, in float ra, float rb)
{
    // sc is the sin/cos of the arc's aperture
	//sc 是圆弧孔径的 sin/cos
    p.x = abs(p.x);
    return ((sc.y*p.x>sc.x*p.y) ? length(p-sc*ra) : 
                                  abs(length(p)-ra)) - rb;
}
// 计算2D旋转矩阵
mat2 Get2DRotationMatrix(float angle) {
    float c = cos(angle);
    float s = sin(angle);
	vec2 V1 = vec2(c, -s);
	vec2 V2 = vec2(s, c);
    return mat2(V1,V2);
}

void fragment() {
	vec2 uv = ((UV / -0.48)) + vec2(1.038,1.038);
    vec2 normalizedCenteredUV = (uv - 0.5) * 2.0;
    float circleSDF = length(uv);
	
    const float fillAnimationFrequency = 4.0;
    const float fillAnimationAmplitude = 0.05;
        
    float fillAnimationPhase = TIME * fillAnimationFrequency;
    float fillAnimation = sin(fillAnimationPhase) * fillAnimationAmplitude;
    
    const float waveFrequency = 2.0;
    const float waveAmplitude = 0.05;
    
    const float waveAnimation = 2.0;
    
    float frontWavePhase = (TIME * waveAnimation) + uv.x;
    float backWavePhase = (TIME * -waveAnimation) + uv.x;
        
    frontWavePhase *= waveFrequency;
    backWavePhase *= waveFrequency;
        
    const float backWavesPhaseOffset = PI;
	
    float frontWaves = sin(frontWavePhase) * waveAmplitude;
    float backWaves = sin(backWavePhase + backWavesPhaseOffset) * waveAmplitude;
	
    float fillProgressAnimationFrequency = 0.5;
    float fillProgressAnimationAmplitude = 0.0;
    
    float fillProgress = fill_value;//填充进度
    
    fillProgress += sin((TIME * fillProgressAnimationFrequency) * PI) * fillProgressAnimationAmplitude;
    fillProgress = (fillProgress - 0.5) * 2.0; 
    
    float frontFill = step(uv.y, (fillAnimation + frontWaves) + fillProgress);
    float backFill = step(uv.y, (-fillAnimation + backWaves) + fillProgress);
    
    backFill = clamp(backFill - frontFill, 0.0, 1.0);
    
    float fillMask = 1.0 - (frontFill + backFill);
    float fill = frontFill + backFill;
    float mask = fill;
    
    vec3 frontFillColour = mix(frontFillInnerColour, frontFillOuterColour, 0);
    const vec3 specularColour = vec3(1.0, 1.0, 0.9);
    const vec3 outerRingGlowColour = vec3(1.0, 0.8, 0.1);
             
    vec3 rgb =
        ((frontFill * frontFillColour) +
        (backFill * backFillColour));
	
    const float backgroundGradientPower = 0.6;
    
    float backgroundGradient = length(normalizedCenteredUV);
    
    backgroundGradient = pow(backgroundGradient, backgroundGradientPower);
    backgroundGradient = smoothstep(0.0, 1.0, backgroundGradient);//第一个参数是背景阴影的亮度
    
    vec3 backgroundGradientInnerColour = vec3(0.13, 0.0, 0.4);
    vec3 backgroundGradientOuterColour = vec3(0.0, 0.0, 0.0);//背景外玻璃颜色
    
    vec3 background = mix(backgroundGradientInnerColour, backgroundGradientOuterColour, backgroundGradient);
    background = clamp(background - (fill), 0.0, 1.0);//背景-（填充+环）,亮度参数1,亮度参数2
         
    const float backgroundStrength = 0.65;//背景强度    
    background *= backgroundStrength;
	
	COLOR = vec4(rgb, mask);
}