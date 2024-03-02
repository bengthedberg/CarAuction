import Heading from "@/app/components/Heading";
import React from "react";

export default function Details({ params }: { params: { id: string } }) {
  return (
    <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
      <div>Details for {params.id}</div>
    </div>
  );
}
