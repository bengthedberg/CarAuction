/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "cdn.pixabay.com",
        pathname: "**",
      },
      {
        protocol: "https",
        hostname: "media.carsandbids.com",
        pathname: "**",
      },
    ],
  },
  output: "standalone"
};

export default nextConfig;
