import { useParamsStore } from "@/hooks/useParamsStore";
import { Button, CustomFlowbiteTheme } from "flowbite-react";
import React from "react";
import { AiOutlineClockCircle, AiOutlineSortAscending } from "react-icons/ai";
import { BsFillStopCircleFill } from "react-icons/bs";

const pageSizeButtons = [4, 8, 12];

const orderButtons = [
  {
    label: "Alphabetical",
    icon: AiOutlineSortAscending,
    value: "make",
  },
  {
    label: "End date",
    icon: AiOutlineClockCircle,
    value: "endingSoon",
  },
  {
    label: "Recently added",
    icon: BsFillStopCircleFill,
    value: "new",
  },
];

const customTheme: CustomFlowbiteTheme["button"] = {
  color: {
    gray: "text-gray-500 bg-white border border-gray-300 enabled:hover:bg-gray-100 enabled:hover:text-cyan-700 focus:text-cyan-700 dark:bg-transparent dark:text-gray-400 dark:border-gray-600 dark:enabled:hover:text-white dark:enabled:hover:bg-gray-700",
    blue: "text-blue-100 bg-blue-700 border border-transparent enabled:hover:bg-gray-100 enabled:hover:text-gray-700 dark:bg-blue-600 dark:hover:bg-blue-700",
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

export default function Filters() {
  const pageSize = useParamsStore((state) => state.pageSize);
  const orderBy = useParamsStore((state) => state.orderBy);
  const setParams = useParamsStore((state) => state.setParams);


  return (
    <div className="flex justify-between items-center mb-4">
      <div>
        <span className="text-sm text-gray-400 mr-2">Order</span>
        <Button.Group>
          {orderButtons.map(({ label, icon: Icon, value }) => (
            <Button
              key={value}
              onClick={() => setParams({ orderBy: value })}
              color={`${orderBy === value ? "blue" : "gray"}`}
              size="custom"
              theme={customTheme}
            >
              <Icon className="mr-3 h-4 w-4" />
              {label}
            </Button>
          ))}
        </Button.Group>
      </div>

      <div>
        <span className="text-sm text-gray-400 mr-2">Page</span>
        <Button.Group>
          {pageSizeButtons.map((value, i) => (
            <Button
              key={i}
              onClick={() => setParams({ pageSize: value })}
              color={`${pageSize === value ? "blue" : "gray"}`}
              size="custom"
              theme={customTheme}
            >
              {value}
            </Button>
          ))}
        </Button.Group>
      </div>
    </div>
  );
}
