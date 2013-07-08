__kernel
    void kernel__error_squared(
    __read_only image2d_t imageA,
    __read_only image2d_t imageB,
    __global uint* errorOutputBuffer
    )
{
    const int2 pos = { get_global_id(0), get_global_id(1) };

    const sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
        CLK_ADDRESS_CLAMP |           //Clamp to zeros
        CLK_FILTER_NEAREST;           //Don't interpolate

    float4 pixelA = read_imagef(imageA, sampler, pos);
    float4 pixelB = read_imagef(imageB, sampler, pos);

    pixelA *= 255.0f;
    pixelB *= 255.0f;

    float r_d = pixelA.x - pixelB.x;
    float g_d = pixelA.y - pixelB.y;
    float b_d = pixelA.z - pixelB.z;

    // r_d^2 + g_d^2 + b_d^2
    float r_d_2 = r_d * r_d;
    float g_d_2 = g_d * g_d;
    float b_d_2 = b_d * b_d;

    float error = r_d_2 + g_d_2 + b_d_2;

	errorOutputBuffer[pos.x + pos.y*get_global_size(0)] = error;
}