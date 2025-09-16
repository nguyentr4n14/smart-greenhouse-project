type Props = { title: string; value?: number; unit?: string };

export function SensorCard({ title, value, unit }: Props) {
    return (
        <div className="rounded-2xl p-4 shadow-md bg-white">
            <div className="font-semibold text-gray-700">{title}</div>
            <div className="mt-2 text-3xl font-bold text-gray-900">
                {value ?? '—'}{' '}
                <span className="text-sm text-gray-500">{unit}</span>
            </div>
        </div>
    );
}
