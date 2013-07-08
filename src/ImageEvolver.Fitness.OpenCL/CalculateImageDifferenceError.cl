#define T_out ulong

__kernel
	void kernel__error_squared(
	__read_only image2d_t imageA,
	__read_only image2d_t imageB,
	__global T_out* errorOutputBuffer
	)
{
	const int2 pos = { get_global_id(0), get_global_id(1) };

	const sampler_t sampler = CLK_NORMALIZED_COORDS_FALSE | //Natural coordinates
		CLK_ADDRESS_CLAMP |           //Clamp to zeros
		CLK_FILTER_NEAREST;           //Don't interpolate

	float3 pixelA = read_imagef(imageA, sampler, pos).xyz;
	float3 pixelB = read_imagef(imageB, sampler, pos).xyz;

	float3 pixel_d = 255.0 * (pixelA - pixelB);

	// r_d^2 + g_d^2 + b_d^2
	T_out error = pixel_d.x*pixel_d.x + pixel_d.y*pixel_d.y + pixel_d.z*pixel_d.z;

    // write back to output buffer, index is (x + y * width)
	errorOutputBuffer[pos.x + pos.y*get_global_size(0)] = error;
}