"use client";

import { useParamsStore } from "@/hooks/useParamsStore";
import React from "react";
import Heading from "./Heading";
import { Button, CustomFlowbiteTheme } from "flowbite-react";

type Props = {
  title?: string;
  subtitle?: string;
  showReset?: boolean;
};

const customTheme: CustomFlowbiteTheme["button"] = {
  color: {
    gray: "text-gray-500 bg-white border border-gray-300 enabled:hover:bg-gray-100 enabled:hover:text-cyan-700 focus:text-cyan-700 dark:bg-transparent dark:text-gray-400 dark:border-gray-600 dark:enabled:hover:text-white dark:enabled:hover:bg-gray-700",
    blue: "text-blue-100 bg-blue-500 border border-transparent enabled:hover:bg-gray-100 enabled:hover:text-gray-700 dark:bg-blue-500 dark:hover:bg-blue-700",
  },
  size: {
    custom: "text-base px-4 py-1",
    xs: "text-xs px-2 py-1",
    sm: "text-xs px-3 py-1.5",
    md: "text-sm px-4 py-2",
    lg: "text-base px-5 py-2.5",
    xl: "text-base px-6 py-3",
  },
};

export default function EmptyFilter({
  title = "No matches for this filter",
  subtitle = "Try changing or resetting the filter",
  showReset,
}: Props) {
  const reset = useParamsStore((state) => state.reset);

  return (
    <div className="h-[40vh] flex flex-col gap-2 justify-center items-center shadow-lg">
      <Heading title={title} subtitle={subtitle} center />
      <div className="mt-4">
        {showReset && (
          <Button color="blue" onClick={reset} size="md" theme={customTheme}>
            Remove Filters
          </Button>
        )}
      </div>
    </div>
  );
}
